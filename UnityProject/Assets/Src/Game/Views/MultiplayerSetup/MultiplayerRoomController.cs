using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GhostGen;

public class MultiplayerRoomController : BaseController 
{
    const byte EVENT_READY_TOGGLE = 1;

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
            _networkManager.onCustomEvent += _onCustomEvent;

            _setupPlayers();
        });
    }

    public override void RemoveView(bool immediately = false)
    {
        _networkManager.onPlayerConnected -= _onPlayerConnectionStatusChanged;
        _networkManager.onPlayerDisconnected -= _onPlayerConnectionStatusChanged;
        _networkManager.onLeftRoom -= _onLeftRoom;
        _networkManager.onCustomEvent -= _onCustomEvent;

        base.RemoveView(immediately);
    }

    private void _addButtonCallbacks(bool isMaster)
    {
        _roomView.leaveButton.onClick.AddListener(_onLeaveRoomButton);

        if (isMaster)
        {
            _roomView.startButton.onClick.AddListener(_onStartGameButton);
        }
        else
        {
            _roomView.readyToggle.onValueChanged.AddListener(_onReadyButton);
        }
    }

    private void _removeButtonCallbacks()
    {
        _roomView.leaveButton.onClick.RemoveAllListeners();
        _roomView.startButton.onClick.RemoveAllListeners();
        _roomView.readyToggle.onValueChanged.RemoveAllListeners();
    }

    private void _onPlayerConnectionStatusChanged(PhotonPlayer newPlayer)
    {
        _removeButtonCallbacks();
        _setupPlayers();
        _addButtonCallbacks(PhotonNetwork.isMasterClient);
    }

    private void _onLeaveRoomButton()
    {
        _roomView.leaveButton.onClick.RemoveListener(_onLeaveRoomButton);
        // Maybe throw up a modal dialog to ask if they are sure?
        PhotonNetwork.LeaveRoom();
    }

    private void _onReadyButton(bool isSelected)
    {
        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent(EVENT_READY_TOGGLE, isSelected, true, options);
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

    private void _onCustomEvent(byte eventCode, object content, int senderId)
    {
        if(eventCode == EVENT_READY_TOGGLE)
        {
            int index = _roomView.GetIndexForPlayerId(senderId);
            if(index >= 0)
            {
                _roomView.SetIsReady(index, (bool)content);
            }
        }
    }

    private void _viewInitialization()
    {
        _roomView.SetTitle(PhotonNetwork.room.Name);
        bool isMaster = PhotonNetwork.isMasterClient;
        _roomView.IsMasterClient(isMaster);
        
        _addButtonCallbacks(isMaster);
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
