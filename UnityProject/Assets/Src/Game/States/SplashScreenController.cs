﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;
using UnityEngine.EventSystems;

public class SplashScreenController : BaseController
{
    private SplashScreen _splashScreen;

    public void Start()
    {
        viewFactory.Create<StarScapeView>("MainMenu/StarScapeView");

        _splashScreen = viewFactory.Create<SplashScreen>("MainMenu/SplashScreen");       

        _splashScreen.onTriggered += OnTriggered;
        _splashScreen.onOutroTransitionEvent += OnOutroFinished;
    }

    private void OnOutroFinished(UIView view)
    {
        Singleton.instance.gameStateMachine.ChangeState(TacoTuesdayState.MAIN_MENU);
    }

    private void OnTriggered(BaseEventData e)
    {
        _splashScreen.onTriggered -= OnTriggered;
        viewFactory.RemoveView(_splashScreen);
    }
}
