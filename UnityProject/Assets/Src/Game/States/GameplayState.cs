using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GhostGen;
using DG.Tweening;

public class GameplayState : IGameState
{
    private PassPlayGameMode    _passPlayGameMode;
    private GameController      _gameController;



    public void Init( GameController p_gameController )
	{       
        Debug.Log ("Entering In GamePlay State");

		_gameController = p_gameController;

        Tween introTween = Singleton.instance.gui.screenFader.FadeIn(1.0f);//fader.DOFade(1.0f, 1.0f);
        introTween.SetDelay(0.25f);

        _passPlayGameMode = new PassPlayGameMode();
        _passPlayGameMode.Start(gotoMainMenu);
       
    }

    
    public void Step( float p_deltaTime )
	{
    }

    public void Exit( GameController p_gameController)
	{
	//	_controller.getUI().rem
		Debug.Log ("Exiting In Intro State");
		//_backButton.onClick.RemoveAllListeners ();
	}

    
    private void gotoMainMenu()
    {
        _passPlayGameMode.CleanUp();
        _gameController.ChangeState(TacoTuesdayState.MAIN_MENU);
    }
    
    
}
