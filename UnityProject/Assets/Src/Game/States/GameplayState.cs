using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GhostGen;
using DG.Tweening;

public class GameplayState : IGameState
{
    private PlayFieldController _playFieldController = new PlayFieldController();
    private GameOverPopupController _gameOverPopupController = new GameOverPopupController();

    private List<PlayerState> _playerList = new List<PlayerState>(4);

    private GameLogic _gameLogic;

	public void Init( GameController p_gameManager )
	{       
        Debug.Log ("Entering In GamePlay State");

		_gameController = p_gameManager;

        Tween introTween = Singleton.instance.viewFactory.screenFader.FadeIn(1.0f);//fader.DOFade(1.0f, 1.0f);
        introTween.SetDelay(0.25f);

        _setupPlayerList();

        _gameLogic = GameLogic.Create(_playerList);
        _playFieldController.Start(_gameLogic, onGameOver);
       
    }

    
    public void Step( float p_deltaTime )
	{
		if (_gotoSplash) 
		{
			_gameController.ChangeState (TacoTuesdayState.INTRO);
			_gotoSplash = false;
		}
    }

    public void Exit( GameController p_gameManager )
	{
	//	_controller.getUI().rem
		Debug.Log ("Exiting In Intro State");
		//_backButton.onClick.RemoveAllListeners ();
	}


	
//------------------- Private Implementation -------------------
//--------------------------------------------------------------
	//private Button 			_backButton;
	//private PlayFieldView		_introView;
	private GameController 	_gameController;
    //private ActiveCustomerSet _customerController;

	private bool _gotoSplash = false;

	private void onBackClick()
	{
		_gotoSplash = true;
	}

    private void onGameOver()
    {
        _gameOverPopupController.Start(_playerList, () =>
        {           
            _playFieldController.RemoveView();
            _gameController.ChangeState(TacoTuesdayState.MAIN_MENU);                                    
        });
    }

    private void _setupPlayerList()
    {
        GameContext context = Singleton.instance.gameManager.gameContext;
        for(int i = 0; i < context.playerNameList.Count; ++i)
        {
            string pName = context.playerNameList[i];
            string name = (string.IsNullOrEmpty(pName)) ? "Player " + i + 1 : pName;
            _playerList.Add(PlayerState.Create(i, name));
        }
    }
}
