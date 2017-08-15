using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassPlayGameMode : IGameModeController
{
    private PassAndPlayFieldController  _playFieldController        = new PassAndPlayFieldController();
    private GameOverPopupController     _gameOverPopupController    = new GameOverPopupController();

    private List<PlayerState>           _playerList                 = new List<PlayerState>(4);


    private GameMatchCore   _gameMatchCore;
    private Action          _onGameOverCallback;

    public void Start(Action gameOverCallback)
    {
        _onGameOverCallback = gameOverCallback;

        GameContext context = Singleton.instance.sessionFlags.gameContext;
        _playerList.Clear();
        _playerList.AddRange(context.playerList);

        _gameMatchCore = GameMatchCore.Create(
            _playerList,
            context.customerDeck, 
            context.ingredientDeck);

        _playFieldController.Start(_gameMatchCore.matchState);
        _setupCallbacks();
    }

    public void Step(double time)
    {

    }

    public void CleanUp()
    {
        _playFieldController.RemoveView();
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
            MatchOverEvent matchOver = MatchOverEvent.Create(_playerList);
            _gameOverPopupController.Start(matchOver.playerRanking, () =>
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

    private void onEndTurn()
    {
        _gameMatchCore.EndPlayerTurn();
    }

    private bool onUndoTurn()
    {
        return _gameMatchCore.UndoLastAction();
    }

    private void _setupCallbacks()
    {
        _playFieldController.onPlayOnCustomer   = onPlayCard;
        _playFieldController.onResolveScore     = onResolveScore;
        _playFieldController.onEndTurn          = onEndTurn;
        _playFieldController.onUndoTurn         = onUndoTurn;
        _playFieldController.onGameOver         = onGameOver;
    }
    
}
