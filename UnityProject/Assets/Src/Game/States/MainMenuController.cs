using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MainMenuController : BaseController 
{
    private MainMenuView _mainMenuView;

	public void Start (MainMenuView view) 
	{
        _setupView(view);
	}

    private void _setupView(MainMenuView view)
    {
        Assert.IsNotNull(view);
        _mainMenuView = view;

        view._startButton.onClick.AddListener(OnStartGame);
        view._creditsButton.onClick.AddListener(OnCredits);
        view._quitButton.onClick.AddListener(OnQuit);
    }
	
    private void OnStartGame()
    {
        _mainMenuView._startButton.onClick.RemoveListener(OnStartGame);

        Singleton.instance.viewFactory.screenFader.FadeOut(0.5f, () =>
        {
            Singleton.instance.viewFactory.RemoveView(_mainMenuView);
            Singleton.instance.gameController.ChangeState(TacoTuesdayState.SETUP_GAME);
        });
        Debug.Log("Start Game!");
    }

    private void OnCredits()
    {
        Debug.Log("Credits!");
    }

    private void OnQuit()
    {
        Application.Quit();
    }
}
