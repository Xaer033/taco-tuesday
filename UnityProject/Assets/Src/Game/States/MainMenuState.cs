using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GhostGen;
using DG.Tweening;

public class MainMenuState : IGameState
{
    private MainMenuController _mainMenuController;
    private StarScapeView _starscapeView;

	public void Init( GameStateMachine stateMachine )
	{
		Debug.Log ("Entering In MainMenu State");
        _mainMenuController = new MainMenuController();

        _starscapeView = GameObject.FindObjectOfType<StarScapeView>();
        if (_starscapeView == null)
        {
            Singleton.instance.gui.viewFactory.CreateAsync<StarScapeView>("MainMenu/StarScapeView", (view) =>
            {
                _starscapeView = view as StarScapeView;
            });
        }

        Singleton.instance.gui.viewFactory.CreateAsync<MainMenuView>("MainMenu/MainMenuView", (view) =>
        {
            Singleton.instance.gui.screenFader.FadeIn(0.35f, () =>
            {
                _mainMenuController.Start(view as MainMenuView);
            });
        });
        
    }
    
    public void Step( float p_deltaTime )
	{
		
    }

    public void Exit( )
	{
	//	_controller.getUI().rem
		Debug.Log ("Exiting In MainMenu State");
        //_backButton.onClick.RemoveAllListeners ();
        Singleton.instance.gui.viewFactory.RemoveView(_starscapeView);
	}
    
}
