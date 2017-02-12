using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePlayerTurn : ICommand
{
    private int _savedPlayerIndex = -1;
    private int _newPlayerIndex = -1;
    private PlayerGroup _playerGroup;
    
    public static ChangePlayerTurn Create(PlayerGroup playerGroup,
        int newPlayerIndex = -1)
    {
        ChangePlayerTurn command = new ChangePlayerTurn();
        command._playerGroup = playerGroup;
        return command;
    }

    private ChangePlayerTurn() { }

    public bool isLinked
    {
        get { return false; }
    }

    public void Execute()
    {
        _savedPlayerIndex = _playerGroup.activePlayer.index;
        if(_newPlayerIndex >= 0)
        {
            _playerGroup.SetActivePlayer(_newPlayerIndex);
        }
        else
        {
            _playerGroup.SetNextActivePlayer();
        }
    }

    public void Undo()
    {
        if(_savedPlayerIndex >= 0)
        {
            _playerGroup.SetActivePlayer(_savedPlayerIndex);
        }
    }
}

