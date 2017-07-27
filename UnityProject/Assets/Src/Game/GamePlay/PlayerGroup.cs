using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class PlayerGroup
{
    public static int kMaxPlayerCount = 4;

    private List<PlayerState> _playerList;
    private int _activePlayerIndex = 0;
    
    public static PlayerGroup Create(List<PlayerState> playerList)
    {
        PlayerGroup group = new PlayerGroup();
        group._playerList = playerList;
        return group;
    }

    private PlayerGroup() { }

    public List<PlayerState> getPlayerList()
    {
        return _playerList;
    }

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

    public PlayerState GetPlayerByIndex(int playerIndex)
    {
        _boundsAssert(playerIndex);
        return _playerList[playerIndex];
    }
    public PlayerState GetPlayerById(int playerId)
    {
        int count = playerCount;
        for (int i = 0; i < count; ++i)
        {
            if(_playerList[i].id == playerId)
            {
                return _playerList[i];
            }
        }

        Debug.LogError("Player id not found: " + playerId);
        return null;
    }
    
    public void SetNextActivePlayer()
    {
        int newIndex = (_activePlayerIndex + 1) % playerCount;
        SetActivePlayer(newIndex);
    }

    public PlayerState GetNextPlayer()
    {
        int newIndex = (_activePlayerIndex + 1) % playerCount;
        return GetPlayerByIndex(newIndex);
    }

    private void _boundsAssert(int index)
    {
        Assert.IsTrue(index >= 0);
        Assert.IsTrue(index < _playerList.Count);
    }
    
}
