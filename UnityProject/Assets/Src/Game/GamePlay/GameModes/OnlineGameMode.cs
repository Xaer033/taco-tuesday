using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            context.isMasterClient, 
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
        if(eventCode == NetworkOpCodes.PLAYER_TURN_COMPLETED)
        {
            EndTurnRequestEvent endTurn = JsonUtility.FromJson<EndTurnRequestEvent>(content as string);
            for(int i = 0; i < endTurn.moveRequestList.Length; ++i)
            {
                MoveRequest move = endTurn.moveRequestList[i];
                Debug.Log(string.Format("End turn event: {0}, {1}, {2}", move.playerIndex, move.handSlot, move.customerSlot));
            }
        }
        else if(eventCode == NetworkOpCodes.BEGIN_NEXT_PLAYER_TURN)
        {
            ChangeTurnEvent changeTurn = JsonUtility.FromJson<ChangeTurnEvent>(content as string);
            Debug.Log("Active Player: " + changeTurn.activePlayerIndex);
        }
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

    private bool onPlayCard(int playerHandIndex, int customerIndex)
    {
        return _gameMatchCore.PlayCardOnCustomer(
                    _gameMatchCore.playerGroup.activePlayer.index,
                    playerHandIndex,
                    customerIndex);
    }

    private bool onResolveScore(int customerIndex)
    {
        return _gameMatchCore.ResolveCustomerCard(
                    customerIndex, 
                    _gameMatchCore.playerGroup.activePlayer.index);
    }

    private void onEndTurn()
    {
        MoveRequest move1 = MoveRequest.Create(_gameMatchCore.playerGroup.activePlayer.index, 2, 3);
        MoveRequest move2 = MoveRequest.Create(_gameMatchCore.playerGroup.activePlayer.index, 4, 1);
        EndTurnRequestEvent endTurnRequest = EndTurnRequestEvent.Create(move1, move2);

        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.MasterClient;

        string requestJson = JsonUtility.ToJson(endTurnRequest);
        PhotonNetwork.RaiseEvent(NetworkOpCodes.PLAYER_TURN_COMPLETED, requestJson, true, options);
        _gameMatchCore.EndPlayerTurn();
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
