using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GhostGen;
using UnityEngine.Assertions;
using System;
using TMPro;

public class MultiplayerRoomView : UIView
{
    public TextMeshProUGUI roomTitle;
    public Button startButton;
    public Button leaveButton;
    public Toggle readyToggle;

    public RoomPlayerItemView playerItemViewPrefab;
    public Transform playerGroup;

    private List<RoomPlayerItemView> _playerItemViewList = new List<RoomPlayerItemView>(4);
    private List<bool> _playerIsReady = new List<bool>(4);
    private string _roomTitle;
    private bool _isMaster;

    void Awake()
    {
        for (int i = 0; i < PlayerGroup.kMaxPlayerCount; ++i)
        {
            RoomPlayerItemView pView = GameObject.Instantiate<RoomPlayerItemView>(playerItemViewPrefab, playerGroup, false);
            pView.gameObject.SetActive(false);
            _playerItemViewList.Add(pView);
            _playerIsReady.Add(false);
        }

        OnIntroTransitionFinished();
    }
    public void IsMasterClient(bool isMaster)
    {
        if (_isMaster != isMaster)
        {
            _isMaster = isMaster;
            invalidateFlag = InvalidationFlag.STATIC_DATA;
        }
    }
    public void SetTitle(string titleText)
    {
        if(_roomTitle != titleText)
        {
            _roomTitle = titleText;
            invalidateFlag = InvalidationFlag.STATIC_DATA;
        }
    }

    public void SetPlayer(int index, PhotonPlayer player)
    {
        Assert.IsTrue(index < _playerItemViewList.Count);

        RoomPlayerItemView pView = _playerItemViewList[index];
        if(player != null)
        {
            pView.playerId = player.ID;
            pView.gameObject.SetActive(true);
            pView.playerName.text = player.NickName;
            pView.checkmark.gameObject.SetActive(false);
        }
        else
        {
            pView.gameObject.SetActive(false);
        }
    }

    public int GetIndexForPlayerId(int playerId)
    {
        int count = _playerItemViewList.Count;
        for(int i = 0; i < count; ++i)
        {
            if(_playerItemViewList[i] == null)
            {
                continue;
            }
            if(playerId == _playerItemViewList[i].playerId)
            {
                return i;
            }
        }
        return -1;
    }

    public void SetIsReady(int index, bool isActive)
    {
        Assert.IsTrue(index < _playerIsReady.Count);

        if (_playerIsReady[index] != isActive)
        {
            _playerIsReady[index] = isActive;
            invalidateFlag = InvalidationFlag.DYNAMIC_DATA;
        }
    }


    protected override void OnViewUpdate()
    {
        if(IsInvalid(InvalidationFlag.STATIC_DATA))
        {
            readyToggle.gameObject.SetActive(!_isMaster);
            startButton.gameObject.SetActive(_isMaster);
            
            roomTitle.text = _roomTitle;
        }

        if (IsInvalid(InvalidationFlag.DYNAMIC_DATA))
        {
            int count = _playerIsReady.Count;
            for (int i = 0; i < count; ++i)
            {
                _playerItemViewList[i].checkmark.gameObject.SetActive(_playerIsReady[i]);
            }
        }
    }
    
}
