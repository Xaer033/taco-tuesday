using System.Collections.Generic;
using GhostGen;
using System;

using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;
using UnityEngine;

public class MultiplayerSetupState : IGameState
{
    private ScreenFader _fader;
    private GameStateMachine _stateMachine;

    private MultiplayerLobbyController _lobbyController;
    private MultiplayerRoomController _roomController;
    private NetworkManager _networkManager;

    public void Init( GameStateMachine stateMachine )
	{
        _stateMachine = stateMachine;

        _fader = Singleton.instance.gui.screenFader;
        _fader.FadeIn(0.35f);

        _networkManager = Singleton.instance.networkManager;
        _networkManager.onCustomEvent += onCustomEvent;
        _networkManager.Connect();

        _lobbyController = new MultiplayerLobbyController();
        _lobbyController.Start(onJoinRoom, onGoToMainMenu);
    }
    
    public void Step( float p_deltaTime )
	{
		
    }

    public void Exit()
	{
        _networkManager.onCustomEvent -= onCustomEvent;

        if (_lobbyController != null )
        {
            _lobbyController.RemoveView();
        }

        if(_roomController != null)
        {
            _roomController.RemoveView();
        }
    }

    private void onGoToMainMenu()
    {
        Singleton.instance.networkManager.Disconnect();

        _fader.FadeOut(0.35f, () =>
        {
            _stateMachine.ChangeState(TacoTuesdayState.MAIN_MENU);
        });
    }

    private void onJoinRoom()
    {
        _lobbyController.RemoveView(true);

        _roomController = new MultiplayerRoomController();
        _roomController.Start(onStartGame, onLeaveRoom);
    }

    private void onStartGame()
    {
        sendGameInitEvent();
    }

    private void sendGameInitEvent()
    {
        int randomSeed = Environment.TickCount;

        CardDeck customerDeck = CardDeck.FromFile("Decks/CustomerDeck");
        customerDeck.Shuffle(randomSeed);
        CardDeck ingredientDeck = CardDeck.FromFile("Decks/IngredientDeck");
        ingredientDeck.Shuffle(randomSeed);

        
        Hashtable contents = new ExitGames.Client.Photon.Hashtable();
        contents.Add("customerDeck",    CardDeck.ToJson(customerDeck, false));
        contents.Add("ingredientDeck",  CardDeck.ToJson(ingredientDeck, false));
        contents.Add("playerList", getSerializablePlayerList(_roomController.GetPlayerList()));

        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent(NetworkOpCodes.INITIAL_GAME_STATE, contents, true, options);
    }

    private string getSerializablePlayerList(PlayerState[] playerStateList)
    {
        if(playerStateList == null)
        {
            Debug.LogError("Player State list is null!");
            return null;
        }

        SerializedPlayerList list = new SerializedPlayerList();

        for(int i = 0; i < playerStateList.Length; ++i)
        {
            if(playerStateList[i] != null)
            {
                list.list[i] = PlayerStateSerializable.Create(playerStateList[i]);
            }
            else
            {
                list.list[i] = null;
            }
        }
        return JsonUtility.ToJson(list);
    }

    private PlayerState[] getPlayerList(string serialList)
    {
        if (string.IsNullOrEmpty(serialList))
        {
            Debug.LogError("Serial State list is null!");
            return null;
        }

        SerializedPlayerList serialPlayerList = JsonUtility.FromJson<SerializedPlayerList>(serialList);
        PlayerState[] list = new PlayerState[serialPlayerList.list.Length];
        for (int i = 0; i < serialPlayerList.list.Length; ++i)
        {
            if(serialPlayerList.list[i] != null)
            {
                PlayerStateSerializable serial = serialPlayerList.list[i];
                list[i] = PlayerState.Create(serial);
            }
            else
            {
                list[i] = null;
            }
        }
        return list;
    }

    private GameContext _createGameContext(CardDeck customerDeck, CardDeck ingredientDeck, PlayerState[] playerList)
    {
        GameContext context = GameContext.Create(GameMode.ONLINE, playerList);
        context.isMasterClient = PhotonNetwork.isMasterClient;
        context.ingredientDeck = ingredientDeck;
        context.customerDeck = customerDeck;
        return context;
    }

    private void onLeaveRoom()
    {
        _roomController.RemoveView(true);

        _lobbyController = new MultiplayerLobbyController();
        _lobbyController.Start(onJoinRoom, onGoToMainMenu);
    }
    

    private void onCustomEvent(byte eventCode, object content, int senderId)
    {
        if (eventCode == NetworkOpCodes.INITIAL_GAME_STATE)
        {
            Hashtable gameInfo = content as Hashtable;
            string  ingredientDeckJson  = gameInfo["ingredientDeck"] as string;
            string  customerDeckJson    = gameInfo["customerDeck"] as string;
            var     playerSerialList    = gameInfo["playerList"] as string;

            CardDeck ingredientDeck     = CardDeck.FromJson(ingredientDeckJson);
            CardDeck customerDeck       = CardDeck.FromJson(customerDeckJson);
            PlayerState[] playerList    = getPlayerList(playerSerialList);

            GameContext context = _createGameContext(customerDeck, ingredientDeck, playerList);
            Singleton.instance.sessionFlags.gameContext = context;
            
            _fader.FadeOut(0.35f, () =>
            {
                _stateMachine.ChangeState(TacoTuesdayState.GAMEPLAY);
            });
        }
    }
}
