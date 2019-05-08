using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GhostGen;
using System;

public class MultiplayerLobbyController : BaseController
{
    private MultiplayerLobbyView _lobbyView;
    private List<Hashtable> _roomLobbyData = new List<Hashtable>();
    private Action _onJoinedRoom;
    private Action _onBack;

    private ListScrollRect _roomListView;
    private int _selectedRoomIndex;

    private NetworkManager _networkManager;

    public void Start(Action onJoinCallback, Action onBackCallback)
    {
        _onJoinedRoom = onJoinCallback;
        _onBack = onBackCallback;

        _selectedRoomIndex = -1;
        _networkManager = Singleton.instance.networkManager;

        viewFactory.CreateAsync<MultiplayerLobbyView>("GameSetup/MultiplayerLobbyView", (popup)=>
        {
            view = _lobbyView = popup as MultiplayerLobbyView;
            _lobbyView._joinButton.onClick.AddListener(onJoinButton);
            _lobbyView._backButton.onClick.AddListener(onBackButton);
            _lobbyView._createButton.onClick.AddListener(onCreateButton);

            _roomListView = _lobbyView._listScrollRect;
            _roomListView.itemRendererFactory = itemRendererFactory;
            _roomListView.onSelectedItem += onRoomClicked;
            
            _roomListView.dataProvider = _getRoomDataProvider();
            
            _networkManager.onReceivedRoomListUpdate += onReceivedRoomListUpdate;
            _networkManager.onJoinedRoom += onJoinedRoom;

        });
    }

    public override void RemoveView(bool immediately = false)
    {
        if(_lobbyView != null)
        {
            _lobbyView._joinButton.onClick.RemoveListener(onJoinButton);
            _lobbyView._backButton.onClick.RemoveListener(onBackButton);
            _lobbyView._createButton.onClick.RemoveListener(onCreateButton);
        }

        if(_roomListView != null)
        {
            _roomListView.onSelectedItem -= onRoomClicked;
        }

        _networkManager.onReceivedRoomListUpdate -= onReceivedRoomListUpdate;
        _networkManager.onJoinedRoom -= onJoinedRoom;

        base.RemoveView(immediately);
    }

    private void onRoomClicked(int index, bool isSelected)
    {
        _lobbyView._joinButton.interactable = _roomListView.selectedIndex >= 0;
        if(isSelected)
        {
            Debug.Log("Item: " + _roomLobbyData[index]["roomName"]);
            _selectedRoomIndex = index;
        }
        else
        {
            _selectedRoomIndex = -1;
        }
    }
    
    private void onJoinButton()
    {
        if(_selectedRoomIndex < 0)
        {
            return;
        }

        _lobbyView._joinButton.onClick.RemoveListener(onJoinButton);

        string roomName = _roomLobbyData[_selectedRoomIndex]["roomName"] as string;
        bool result = PhotonNetwork.JoinRoom(roomName);

        Debug.Log(string.Format("Joining room: {0} with result: {1}", roomName, result));  
    }

    private void onCreateButton()
    {
        _lobbyView._createButton.onClick.RemoveListener(onBackButton);
        Debug.Log("Create a room");

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)PlayerGroup.kMaxPlayerCount;

        string roomName = "Room_" + UnityEngine.Random.Range(0, 10000);
        bool result = PhotonNetwork.CreateRoom(roomName, options, null);
        Debug.Log("Create Room Result: " + result);
    }

    private void onBackButton()
    {
        _lobbyView._backButton.onClick.RemoveListener(onBackButton);
        if (_onBack != null)
        {
            _onBack();
        }
    }

    private GameObject itemRendererFactory(int itemType, Transform parent)
    {
        RoomItemView view = GameObject.Instantiate(_lobbyView._listItemPrefab, parent, false);
        view.toggle.group = _lobbyView._toggleGroup;
        return view.gameObject;
    }

    private void onReceivedRoomListUpdate()
    {
        _roomListView.dataProvider = _getRoomDataProvider();
        PhotonNetwork.player.NickName = string.Format("PL-{0}", UnityEngine.Random.Range(0, 1000));//SystemInfo.deviceName + "_" + UnityEngine.Random.Range(0, 2000);
    }

    private void onJoinedRoom()
    {
        if (_onJoinedRoom != null)
        {
            _onJoinedRoom();
        }
    }

    private List<Hashtable> _getRoomDataProvider()
    {
        _roomLobbyData.Clear();

        Debug.Log("Is inside Lobby: " + PhotonNetwork.insideLobby) ;
        RoomInfo[] roomList = PhotonNetwork.GetRoomList();
        int roomCount = roomList.Length;
        for(int i = 0; i < roomCount; ++i)
        {
            Hashtable roomData = new Hashtable();

            RoomInfo info = roomList[i];
            if(!info.IsVisible) { continue; }

            roomData.Add("roomName", info.Name);
            roomData.Add("playerCount", string.Format("{0}/{1}", info.PlayerCount, info.MaxPlayers));
            _roomLobbyData.Add(roomData);
        }

        return _roomLobbyData;
    }
}
