﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GhostGen;

public class MultiplayerRoomController : BaseController 
{
    private MultiplayerRoomView _roomView;
    private NetworkManager _networkManager;
    private Action _onBackCallback;
    private Action _onStartGameCallback;

    public void Start(Action onStartGame, Action onBack)
    {
        _onStartGameCallback = onStartGame;
        _onBackCallback = onBack;

        _networkManager = Singleton.instance.networkManager;
        viewFactory.CreateAsync<MultiplayerRoomView>("GameSetup/MultiplayerRoomView", (v) =>
        {
            view = _roomView = v as MultiplayerRoomView;
            _viewInitialization();

            _networkManager.onPlayerConnected += _onPlayerConnectionStatusChanged;
            _networkManager.onPlayerDisconnected += _onPlayerConnectionStatusChanged;
            _networkManager.onLeftRoom += _onLeftRoom;

            _setupPlayers();
        });
    }

    public override void RemoveView(bool immediately = false)
    {
        _networkManager.onPlayerConnected -= _onPlayerConnectionStatusChanged;
        _networkManager.onPlayerDisconnected -= _onPlayerConnectionStatusChanged;
        _networkManager.onLeftRoom -= _onLeftRoom;

        base.RemoveView(immediately);
    }

    private void _onPlayerConnectionStatusChanged(PhotonPlayer newPlayer)
    {
        _setupPlayers();
    }

    private void _onLeaveRoomButton()
    {
        // Maybe throw up a modal dialog to ask if they are sure?
        _roomView.leaveButton.onClick.RemoveListener(_onLeaveRoomButton);
        PhotonNetwork.LeaveRoom();
    }

    private void _onLeftRoom()
    {
        if (_onBackCallback != null)
        {
            _onBackCallback();
        }
    }

    private void _onStartGameButton()
    {
        if(_onStartGameCallback != null)
        {
            _onStartGameCallback();
        }
    }

    private void _viewInitialization()
    {
        _roomView.leaveButton.onClick.AddListener(_onLeaveRoomButton);

        _roomView.SetTitle(PhotonNetwork.room.Name);
        bool isMaster = PhotonNetwork.isMasterClient;
        _roomView.IsMasterClient(isMaster);

        if(isMaster)
        {
            _roomView.startButton.onClick.AddListener(_onStartGameButton);
        }
        else
        {
            _roomView.readyToggle.onValueChanged.AddListener((isSelected) => { });
        }
    }

    private void _setupPlayers()
    {
        PhotonPlayer[] playerList = PhotonNetwork.playerList;
        int count = playerList.Length;
        for(int i = 0; i < PlayerGroup.kMaxPlayerCount; ++i)
        {
            if(i < count)
            {
                _roomView.SetPlayer(i, playerList[i]);
            }
            else
            {
                _roomView.SetPlayer(i, null);
            }
        }

        _roomView.IsMasterClient(PhotonNetwork.isMasterClient);
    }
}
