using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GhostGen;
using DG.Tweening;

public class MainMenuState : IGameState
{
	private GameController 	_gameController;
    private MainMenuController _mainMenuController;

	public void Init( GameController gameController )
	{
		Debug.Log ("Entering In MainMenu State");
        _gameController = gameController;

        _mainMenuController = new MainMenuController();
        Singleton.instance.viewFactory.CreateAsync<MainMenuView>("MainMenu/MainMenuView", (view) =>
        {
            Singleton.instance.viewFactory.screenFader.FadeIn(0.35f, () =>
            {
                _mainMenuController.Start(view as MainMenuView);
            });
        });
    }
    
    public void Step( float p_deltaTime )
	{
		
    }

    public void Exit( GameController gameController )
	{
	//	_controller.getUI().rem
		Debug.Log ("Exiting In MainMenu State");
		//_backButton.onClick.RemoveAllListeners ();
	}
    
}
