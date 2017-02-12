using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class PlayerGroup
{

    private List<PlayerState> _playerList;
    private int _activePlayerIndex = 0;
    
    public static PlayerGroup Create(List<PlayerState> playerList)
    {
        PlayerGroup group = new PlayerGroup();
        group._playerList = playerList;
        return group;
    }

    private PlayerGroup() { }

    public PlayerState activePlayer
    {
        get { return _playerList[_activePlayerIndex]; }
    }

    public int playerCount
    {
        get { return _playerList.Count; }
    }

    public void SetActivePlayer(int playerIndex)
    {
        _boundsAssert(playerIndex);
        _activePlayerIndex = playerIndex;
    }

    public PlayerState GetPlayer(int playerIndex)
    {
        _boundsAssert(playerIndex);
        return _playerList[playerIndex];
    }

    public void SetNextActivePlayer()
    {
        int newIndex = (_activePlayerIndex + 1) % playerCount;
        SetActivePlayer(newIndex);
    }

    private void _boundsAssert(int index)
    {
        Assert.IsTrue(index >= 0);
        Assert.IsTrue(index < _playerList.Count);
    }
    
}
