using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class OnlineGameMode : IGameModeController
{
    private const int kRandomSeed = 100;

    private NormalPlayFieldController   _playFieldController        = new NormalPlayFieldController();
    private GameOverPopupController     _gameOverPopupController    = new GameOverPopupController();

    private List<PlayerState>           _playerList                 = new List<PlayerState>(PlayerGroup.kMaxPlayerCount);

    private GameMatchCore   _gameMatchCore;
    private Action          _onGameOverCallback;

    private NetworkManager  _networkManager;

    public void Start( Action gameOverCallback)
    {
        _networkManager = Singleton.instance.networkManager;
        _networkManager.onCustomEvent += onCustomEvent;

        _onGameOverCallback = gameOverCallback;

        GameContext context = Singleton.instance.sessionFlags.gameContext;
        _playerList.Clear();
        _playerList.AddRange(context.playerList);
        
        _gameMatchCore = GameMatchCore.Create(
            _playerList, 
            context.customerDeck, 
            context.ingredientDeck);

        _playFieldController.Start(_gameMatchCore.matchState);
        _setupPlayFieldCallbacks();
    }

    public void CleanUp()
    {
        _playFieldController.RemoveView();
        _networkManager.onCustomEvent -= onCustomEvent;
        _networkManager.Disconnect();
    }

    private void onCustomEvent(byte eventCode, object content, int senderId)
    {
        bool isSenderThisPlayer = senderId == PhotonNetwork.player.ID;
        if (PhotonNetwork.isMasterClient && eventCode == NetworkOpCodes.PLAYER_TURN_COMPLETED)
        {
            EndTurnRequestEvent endTurn = JsonUtility.FromJson<EndTurnRequestEvent>(content as string);
            if(isSenderThisPlayer)
            {
                sendTurnOverEvent(endTurn.moveRequestList);
            }
            else if(attemptMoves(senderId, endTurn.moveRequestList))
            {
                sendTurnOverEvent(endTurn.moveRequestList);
            }
        }
        else if(eventCode == NetworkOpCodes.BEGIN_NEXT_PLAYER_TURN)
        {
            ChangeTurnEvent changeTurn = JsonUtility.FromJson<ChangeTurnEvent>(content as string);
            Debug.Log("Active Player: " + changeTurn.activePlayerIndex);

            int previousPlayerId = _gameMatchCore.playerGroup.GetPlayerByIndex(changeTurn.previousPlayerIndex).id;
            if (!PhotonNetwork.isMasterClient && previousPlayerId != PhotonNetwork.player.ID)
            {
                attemptMoves(previousPlayerId, changeTurn.moveRequestList);
            }

            simulateOtherPlayerMakingMoves(() =>
            {
                _gameMatchCore.EndPlayerTurn();
                _gameMatchCore.playerGroup.SetActivePlayer(changeTurn.activePlayerIndex);
                _gameMatchCore.ClearCommandBuffer();

                _playFieldController.RefreshHandView();
                _playFieldController.RefreshCustomersView();

                _playFieldController.SetPlayerScoreView(
                    changeTurn.previousPlayerIndex, 
                    changeTurn.prevPlayerScore);
            });
        }
    }

    private void simulateOtherPlayerMakingMoves(Action onComplete)
    {
        //Totally do more shit here with playField in the future
        if(onComplete != null)
        {
            onComplete();
        }
    }

    private bool attemptMoves(int playerId, MoveRequest[] moveRequests)
    {
        Assert.IsNotNull(moveRequests);

        int count = moveRequests.Length;
        for(int i = 0; i < count; ++i)
        {
            MoveRequest move = moveRequests[i];
            if (move == MoveRequest.Invalid())
            {
                continue;
            }

            bool result = onPlayCard(move);
            if(!result)
            {

                PhotonPlayer badGuy = _networkManager.GetPlayerById(playerId);
                Debug.LogErrorFormat("Player: {0}:{1} tried to make illegal move! {2}:{3}:{4}",
                    playerId, 
                    badGuy.NickName,
                    move.playerIndex,
                    move.handSlot,
                    move.customerSlot);
                return false;
            }
            else
            {
                onResolveScore(move.customerSlot);
            }
        }

        return true;
    }

    private void sendTurnOverEvent(MoveRequest[] moveList)
    {
        int currentPl = _gameMatchCore.playerGroup.activePlayer.index;
        int nextPl = _gameMatchCore.playerGroup.GetNextPlayer().index;
        int score = _gameMatchCore.playerGroup.activePlayer.score;

        ChangeTurnEvent changeTurn = ChangeTurnEvent.Create(currentPl, nextPl, score, moveList );
        string eventJson = JsonUtility.ToJson(changeTurn);
        
        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.All;

        PhotonNetwork.RaiseEvent(NetworkOpCodes.BEGIN_NEXT_PLAYER_TURN, eventJson, true, options);
    }

    private void onGameOver(bool gameOverPopup = true)
    {
        if (!gameOverPopup)
        {
            Singleton.instance.gui.screenFader.FadeOut(0.5f, () =>
            {
                if (_onGameOverCallback != null) { _onGameOverCallback(); }
            });
        }
        else
        {
            _gameOverPopupController.Start(_playerList, () =>
            {
                if (_onGameOverCallback != null) { _onGameOverCallback(); }
            });
        }
    }

    private bool onPlayCard(MoveRequest move)
    {
        return _gameMatchCore.PlayCardOnCustomer(move);
    }

    private bool onResolveScore(int customerIndex)
    {
        return _gameMatchCore.ResolveCustomerCard(
                    customerIndex, 
                    _gameMatchCore.playerGroup.activePlayer.index);
    }

    private void onEndTurn(MoveRequest[] moveList)
    {
        EndTurnRequestEvent endTurnRequest = EndTurnRequestEvent.Create(PhotonNetwork.player.ID, moveList);

        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.MasterClient;

        string requestJson = JsonUtility.ToJson(endTurnRequest);
        PhotonNetwork.RaiseEvent(NetworkOpCodes.PLAYER_TURN_COMPLETED, requestJson, true, options);
    }

    private bool onUndoTurn()
    {
        return _gameMatchCore.UndoLastAction();
    }

    private void _setupPlayFieldCallbacks()
    {
        _playFieldController.onPlayOnCustomer   = onPlayCard;
        _playFieldController.onResolveScore     = onResolveScore;
        _playFieldController.onEndTurn          = onEndTurn;
        _playFieldController.onUndoTurn         = onUndoTurn;
        _playFieldController.onGameOver         = onGameOver;
    }
}
