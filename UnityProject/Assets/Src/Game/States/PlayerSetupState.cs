using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GhostGen;
using DG.Tweening;

public class PlayerSetupState : IGameState
{
    private PassPlaySetupController _passPlaySetupController;
    private ScreenFader _fader;
    private GameController _controller;

	public void Init( GameController gameController )
	{
        _controller = gameController;

        _passPlaySetupController = new PassPlaySetupController();
        _passPlaySetupController.Start(onPassSetupStart, onPassSetupCancel);

        _fader = Singleton.instance.gui.screenFader;
        _fader.FadeIn(0.35f);       
    }
    
    public void Step( float p_deltaTime )
	{
		
    }

    public void Exit(GameController gameController)
	{
        _passPlaySetupController.RemoveView();
	}

    private void onPassSetupStart()
    {
        List<string> pNames = _passPlaySetupController.GetNameList();
        GameContext context = GameContext.Create(GameType.PASS_AND_PLAY, pNames);
        Singleton.instance.sessionFlags.gameContext = context;

        _fader.FadeOut(0.35f, () =>
        {
            _controller.ChangeState(TacoTuesdayState.GAMEPLAY);
        });
    }

    private void onPassSetupCancel()
    {
        _fader.FadeOut(0.35f, () =>
        {
            _controller.ChangeState(TacoTuesdayState.MAIN_MENU);
        });
    }
}
