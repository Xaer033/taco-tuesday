using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GhostGen;
using DG.Tweening;

public class PlayerSetupState : IGameState
{
    private PassPlaySetupController _passPlaySetupController;

	public void Init( GameController gameController )
	{
        _passPlaySetupController = new PassPlaySetupController();
        _passPlaySetupController.Start(onPassSetupStart, onPassSetupCancel);

        ViewFactory viewFactory = Singleton.instance.viewFactory;
        viewFactory.screenFader.FadeIn(0.35f);       
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
        GameContext context = GameContext.Create(GameContext.GameType.PASS_AND_PLAY);
        context.playerNameList = _passPlaySetupController.GetNameList();
        Singleton.instance.gameManager.gameContext = context;

        Singleton.instance.viewFactory.screenFader.FadeOut(0.35f, () =>
        {
            Singleton.instance.gameController.ChangeState(TacoTuesdayState.GAMEPLAY);
        });
    }

    private void onPassSetupCancel()
    {
        Singleton.instance.viewFactory.screenFader.FadeOut(0.35f, () =>
        {
            Singleton.instance.gameController.ChangeState(TacoTuesdayState.MAIN_MENU);
        });
    }
}
