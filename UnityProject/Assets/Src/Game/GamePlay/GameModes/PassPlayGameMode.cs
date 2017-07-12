using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassPlayGameMode : IGameModeController
{
    private PlayFieldController     _playFieldController        = new PlayFieldController();
    private GameOverPopupController _gameOverPopupController    = new GameOverPopupController();

    private List<PlayerState>       _playerList                 = new List<PlayerState>(4);


    private GameMatchCore   _gameMatchCore;
    private Action          _onGameOverCallback;

    public void Start(Action gameOverCallback)
    {
        _onGameOverCallback = gameOverCallback;

        _setupPlayerList();

        _gameMatchCore = GameMatchCore.Create(_playerList);

        _playFieldController.Start(_gameMatchCore.matchState);
        _setupCallbacks();
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
            _gameOverPopupController.Start(_playerList, () =>
            {
                if (_onGameOverCallback != null) { _onGameOverCallback(); }
            });
        }
    }
    
    private void _setupPlayerList()
    {
        GameContext context = Singleton.instance.sessionFlags.gameContext;
        for (int i = 0; i < context.playerNameList.Count; ++i)
        {
            string pName = context.playerNameList[i];
            string name = (string.IsNullOrEmpty(pName)) ? (i + 1).ToString() : pName;
            _playerList.Add(PlayerState.Create(i, name));
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
