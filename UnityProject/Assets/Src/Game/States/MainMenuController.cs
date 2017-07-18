using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MainMenuController : BaseController 
{
    private MainMenuView    _mainMenuView;
    private GuiManager      _gui;

	public void Start (MainMenuView view) 
	{
        _gui = Singleton.instance.gui;
        _setupView(view);
	}

    private void _toggleSubmenu(bool submenu)
    {
        if(_mainMenuView != null)
        {
            _mainMenuView.buttonGroupOne._container.SetActive(!submenu);
            _mainMenuView.buttonGroupTwo._container.SetActive(submenu);
        }
    }
    private void _setupView(MainMenuView view)
    {
        Assert.IsNotNull(view);
        _mainMenuView = view;

        _toggleSubmenu(false);

        view.buttonGroupOne._startButton.onClick.AddListener(onStartGameSubMenu);
        view.buttonGroupOne._creditsButton.onClick.AddListener(onCredits);
        view.buttonGroupOne._quitButton.onClick.AddListener(onQuit);

        view.buttonGroupTwo._singlePlayerBtn.onClick.AddListener(onSinglePlayer);
        view.buttonGroupTwo._passAndPlayBtn.onClick.AddListener(onPassAndPlay);
        view.buttonGroupTwo._onlineBtn.onClick.AddListener(onOnline);
        view.buttonGroupTwo._backBtn.onClick.AddListener(onBack);
    }
	
    private void onStartGameSubMenu()
    {
        //_mainMenuView.buttonGroupOne._startButton.onClick.RemoveListener(OnStartGame);
        _toggleSubmenu(true);
    }

    private void onSinglePlayer()
    {
        Debug.Log("No Singleplayer yet");
    }

    private void onPassAndPlay()
    {
        _mainMenuView.buttonGroupTwo._passAndPlayBtn.onClick.RemoveListener(onPassAndPlay);

        _gui.screenFader.FadeOut(0.5f, () =>
        {
            _gui.viewFactory.RemoveView(_mainMenuView);
            Singleton.instance.gameStateMachine.ChangeState(TacoTuesdayState.PASS_PLAY_SETUP_GAME);
        });
        Debug.Log("Start Game!");
    }

    private void onOnline()
    {
        _gui.screenFader.FadeOut(0.5f, () =>
        {
            _gui.viewFactory.RemoveView(_mainMenuView);
            Singleton.instance.gameStateMachine.ChangeState(TacoTuesdayState.MULTIPLAYER_SETUP_GAME);
        });
    }

    private void onBack()
    {
        _toggleSubmenu(false);
    }

    private void onCredits()
    {
        Debug.Log("Credits!");
    }

    private void onQuit()
    {
        Application.Quit();
    }
}
