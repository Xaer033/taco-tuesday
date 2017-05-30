using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;
using UnityEngine.EventSystems;

public class SplashScreenController : BaseController
{
    private SplashScreen _splashScreen;

    public void Start()
    {
        _splashScreen = viewFactory.Create<SplashScreen>("MainMenu/SplashScreen", viewFactory.canvas.transform);       

        _splashScreen.onTriggered += OnTriggered;
        _splashScreen.onOutroTransitionEvent += OnOutroFinished;
    }

    private void OnOutroFinished(UIView view)
    {
        GameManager.instance.gameController.ChangeState(TacoTuesdayState.MAIN_MENU);
    }

    private void OnTriggered(BaseEventData e)
    {
        _splashScreen.onTriggered -= OnTriggered;
        viewFactory.RemoveView(_splashScreen);
    }
}
