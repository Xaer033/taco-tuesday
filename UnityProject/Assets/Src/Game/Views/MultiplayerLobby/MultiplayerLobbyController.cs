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
    private Action _onJoin;
    private Action _onBack;

    private ListScrollRect _roomListView;

    public void Start(Action onJoinCallback, Action onBackCallback)
    {
        
        _onJoin = onJoinCallback;
        _onBack = onBackCallback;

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

            //Singleton.instance.networkManager.onCreatedRoom += onCreatedRoom;
            Singleton.instance.networkManager.onReceivedRoomListUpdate += onReceivedRoomListUpdate;
        });
    }

    public override void RemoveView()
    {
        _lobbyView._joinButton.onClick.RemoveListener(onJoinButton);
        _lobbyView._backButton.onClick.RemoveListener(onBackButton);
        _lobbyView._createButton.onClick.RemoveListener(onCreateButton);

        _roomListView.onSelectedItem -= onRoomClicked;


        Singleton.instance.networkManager.onReceivedRoomListUpdate -= onReceivedRoomListUpdate;
        base.RemoveView();
    }

    private void onRoomClicked(int index, bool isSelected)
    {
        _lobbyView._joinButton.interactable = _roomListView.selectedIndex >= 0;
        if(isSelected)
        {
            Debug.Log("Item: " + _roomLobbyData[index]["roomName"]);
        }
    }
    
    private void onJoinButton()
    {
        Debug.Log("Join a room: ");
        if(_onJoin != null)
        {
            _onJoin();
        }
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
