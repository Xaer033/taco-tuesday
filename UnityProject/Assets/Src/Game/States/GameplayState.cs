using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GhostGen;
using DG.Tweening;

public class GameplayState : IGameState
{
    private IGameModeController _gameModeController;
    private GameStateMachine    _stateMachine;

    public void Init( GameStateMachine stateMachine )
	{       
        Debug.Log ("Entering In GamePlay State");

		_stateMachine = stateMachine;

        Tween introTween = Singleton.instance.gui.screenFader.FadeIn(1.0f);
        introTween.SetDelay(0.25f);

        _gameModeController = getGameModeController();
        _gameModeController.Start(gotoMainMenu);
    }

    
    public void Step( float p_deltaTime )
	{

    }

    public void Exit()
	{
		Debug.Log ("Exiting In Intro State");

        _gameModeController.CleanUp();
    }

    
    private void gotoMainMenu()
    {
        _stateMachine.ChangeState(TacoTuesdayState.MAIN_MENU);
    }

    private IGameModeController getGameModeController()
    {
        GameContext context = Singleton.instance.sessionFlags.gameContext;
        switch(context.gameMode)
        {
            case GameMode.PASS_AND_PLAY:    return new PassPlayGameMode();
            case GameMode.ONLINE:           return new OnlineGameMode();
            case GameMode.SINGLE_PLAYER:    return null;
        }
        Debug.LogErrorFormat("Not supported gametype {0}", context.gameMode);
        return null;
    }
}
