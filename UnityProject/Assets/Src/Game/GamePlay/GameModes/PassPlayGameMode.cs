using System;
using System.Collections.Generic;
using UnityEngine;

public class PassPlayGameMode : IGameModeController
{
    private PlayFieldController     _playFieldController        = new PlayFieldController();
    private GameOverPopupController _gameOverPopupController    = new GameOverPopupController();

    private List<PlayerState>       _playerList                 = new List<PlayerState>(4);


    private GameLogic   _gameLogic;
    private Action      _onGameOverCallback;

    public void Start(Action gameOverCallback)
    {
        _onGameOverCallback = gameOverCallback;

        _setupPlayerList();

        _gameLogic = GameLogic.Create(_playerList);
        _gameLogic.onPlayOnCustomer += onPlayCard;
        _gameLogic.onResolveScore += onResolveScore;
        _gameLogic.onEndTurn += onEndTurn;

        _playFieldController.Start(_gameLogic, onGameOver);
    }

    public void CleanUp()
    {
        _playFieldController.RemoveView();

        _gameLogic.onPlayOnCustomer -= onPlayCard;
        _gameLogic.onResolveScore -= onResolveScore;
        _gameLogic.onEndTurn -= onEndTurn;
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

    private void onPlayCard()
    {
        Debug.Log("On Play Card");
    }

    private void onResolveScore(bool result)
    {
        Debug.Log("On Resolve Card");
    }

    private void onEndTurn()
    {
        Debug.Log("On End Turn");
    }
}
