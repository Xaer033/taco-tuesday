using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GhostGen;
using DG.Tweening;

public class MultiplayerSetupState : IGameState
{
    private ScreenFader _fader;
    private GameStateMachine _stateMachine;

    private MultiplayerLobbyController _lobbyController;
    private MultiplayerRoomController _roomController;

    public void Init( GameStateMachine stateMachine )
	{
        _stateMachine = stateMachine;

        _fader = Singleton.instance.gui.screenFader;
        _fader.FadeIn(0.35f);

        Singleton.instance.networkManager.Connect();

        _lobbyController = new MultiplayerLobbyController();
        _lobbyController.Start(onJoinRoom, onGoToMainMenu);
    }
    
    public void Step( float p_deltaTime )
	{
		
    }

    public void Exit()
	{
        _lobbyController.RemoveView();
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
        Debug.Log("Start game or somethin'");
    }

    private void onLeaveRoom()
    {
        _roomController.RemoveView(true);

        _lobbyController = new MultiplayerLobbyController();
        _lobbyController.Start(onJoinRoom, onGoToMainMenu);
    }
}
