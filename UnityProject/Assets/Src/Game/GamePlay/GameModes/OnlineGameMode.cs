﻿using System;
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
    private List<MoveResult>            _moveResultList             = new List<MoveResult>();
    private List<MoveRequest>           _recycleRequest             = new List<MoveRequest>();

    private GameMatchCore   _gameMatchCore;
    private Action          _onGameOverCallback;

    private NetworkManager  _networkManager;

    public void Start(Action gameOverCallback)
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
            
            if (isSenderThisPlayer)
            {
                sendTurnOverEvent(endTurn.moveRequestList);
            }
            else if(attemptMoves(senderId, endTurn.moveRequestList, true))
            {
                sendTurnOverEvent(endTurn.moveRequestList);
            }

            if (_gameMatchCore.isGameOver)
            {
                sendGameOverEvent();
            }
        }
        else if(eventCode == NetworkOpCodes.BEGIN_NEXT_PLAYER_TURN)
        {
            ChangeTurnEvent changeTurn = JsonUtility.FromJson<ChangeTurnEvent>(content as string);
            Debug.Log("Active Player: " + changeTurn.activePlayerIndex);

            int previousPlayerId = _gameMatchCore.playerGroup.GetPlayerByIndex(changeTurn.previousPlayerIndex).id;

            bool isSamePlayer = previousPlayerId == PhotonNetwork.player.ID;
            if (!PhotonNetwork.isMasterClient && !isSamePlayer)
            {
                attemptMoves(previousPlayerId, changeTurn.moveRequestList, true);
                simulateOtherPlayerMakingMoves(changeTurn.moveResultList, () => onCompleteOtherPlayerAnimation(changeTurn));
            }
            else if(!isSamePlayer)
            {
                simulateOtherPlayerMakingMoves(changeTurn.moveResultList, () => onCompleteOtherPlayerAnimation(changeTurn));
            }
            else
            {
                onCompleteOtherPlayerAnimation(changeTurn);
            }
            _moveResultList.Clear();
        }
        else if(eventCode == NetworkOpCodes.MATCH_OVER)
        {
            MatchOverEvent matchOver = JsonUtility.FromJson<MatchOverEvent>(content as string);
           
            _gameOverPopupController.Start(matchOver.playerRanking, () =>
            {
                if (_onGameOverCallback != null) { _onGameOverCallback(); }
            });
        }
    }

    private void simulateOtherPlayerMakingMoves(List<MoveResult> resultList, Action onComplete)
    {
        _playFieldController.AnimateOtherPlayerMoves(resultList, ()=>
        {
            if(onComplete != null)
            {
                onComplete();
            }
        }); 
    }

    private void onCompleteOtherPlayerAnimation(ChangeTurnEvent changeTurnEvent)
    {
        _gameMatchCore.EndPlayerTurn();
        _gameMatchCore.playerGroup.SetActivePlayer(changeTurnEvent.activePlayerIndex);
        _gameMatchCore.ClearCommandBuffer();

        _playFieldController.RefreshHandView();
        _playFieldController.RefreshCustomersView();

        _playFieldController.SetPlayerScoreView(
            changeTurnEvent.previousPlayerIndex,
            changeTurnEvent.prevPlayerScore);
    }

    private bool attemptMoves(int playerId, List<MoveRequest> moveRequests, bool shouldResolveScore)
    {
        Assert.IsNotNull(moveRequests);

        int count = moveRequests.Count;
        for(int i = 0; i < count; ++i)
        {
            MoveRequest request = moveRequests[i];
            if (request == MoveRequest.Invalid())
            {
                continue;
            }

            PlayerState player = _gameMatchCore.matchState.playerGroup.GetPlayerByIndex(request.playerIndex);

            int scoreDelta = 0;
            int oldScore = player.score;

            MoveResult moveResult = new MoveResult();
            moveResult.usedIngredient = player.hand.GetCard(request.handSlot).id;
            Debug.LogError("Setting Ingredient: " + moveResult.usedIngredient);

            bool result = _gameMatchCore.PlayCardOnCustomer(request);
            if(!result)
            {
                PhotonPlayer badGuy = _networkManager.GetPlayerById(playerId);
                Debug.LogErrorFormat("Player: {0}:{1} tried to make illegal move! {2}:{3}:{4}",
                    playerId, 
                    badGuy.NickName,
                    request.playerIndex,
                    request.handSlot,
                    request.customerSlot);
                return false;
            }
            
            if(shouldResolveScore)
            {
                onResolveScore(request.customerSlot);
                scoreDelta = player.score - oldScore;
            }

            moveResult.request = request;
            moveResult.addedScore = scoreDelta;

            _moveResultList.Add(moveResult);
        }
        
        return true;
    }

    private void sendTurnOverEvent(List<MoveRequest> requestList)
    {
        int currentPl = _gameMatchCore.playerGroup.activePlayer.index;
        int nextPl = _gameMatchCore.playerGroup.GetNextPlayer().index;
        int score = _gameMatchCore.playerGroup.activePlayer.score;

        ChangeTurnEvent changeTurn = ChangeTurnEvent.Create(currentPl, nextPl, score, requestList, _moveResultList);
        string eventJson = JsonUtility.ToJson(changeTurn);
        
        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.All;

        PhotonNetwork.RaiseEvent(NetworkOpCodes.BEGIN_NEXT_PLAYER_TURN, eventJson, true, options);

    }

    private void sendGameOverEvent()
    {
        List<PlayerState>   playerList = _gameMatchCore.matchState.playerGroup.getPlayerList();
        MatchOverEvent      matchEvent = MatchOverEvent.Create(playerList);

        string eventJson = JsonUtility.ToJson(matchEvent);

        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.All;

        PhotonNetwork.RaiseEvent(NetworkOpCodes.MATCH_OVER, eventJson, true, options);
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

        }
    }

    private bool onPlayCard(MoveRequest move)
    {
        _recycleRequest.Clear();
        _recycleRequest.Add(move);
        int playerIndex = _gameMatchCore.playerGroup.activePlayer.index;
        return attemptMoves(playerIndex, _recycleRequest, false);
    }

    private bool onResolveScore(int customerIndex)
    {
        return _gameMatchCore.ResolveCustomerCard(
                    customerIndex, 
                    _gameMatchCore.playerGroup.activePlayer.index);
    }

    private void onEndTurn(List<MoveRequest> moveList)
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
