using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GhostGen;
using DG.Tweening;

public class IntroState : IGameState
{
    private PlayFieldController _playFieldController = new PlayFieldController();
    private List<PlayerState> _playerList = new List<PlayerState>(4);

    private GameLogic _gameLogic;

	public void Init( GameController p_gameManager )
	{
		Debug.Log ("Entering In Intro State");
        DOTween.Init(true, true, LogBehaviour.ErrorsOnly);

		_gameController = p_gameManager;

        _playerList.Add(PlayerState.Create(0, "John"));
        _playerList.Add(PlayerState.Create(1, "Poop"));
        _playerList.Add(PlayerState.Create(2, "Adith"));
        _playerList.Add(PlayerState.Create(3, "Maud'Dib"));

        _gameLogic = GameLogic.Create(_playerList);
        _playFieldController.Start(_gameLogic);
       
    }

    
    public void Step( float p_deltaTime )
	{
		if (_gotoSplash) 
		{
			_gameController.ChangeState (TacoTuesdayState.Intro);
			_gotoSplash = false;
		}

        //Transform camTransform = Camera.main.transform;
        //Vector3 lookPos = camTransform.position + camTransform.forward * 10.0f;

        //Vector3 grav = -Input.gyro.gravity;
        //grav.x = -grav.x;
        //Camera.main.transform.LookAt(lookPos, grav.normalized);
    }

    public void Exit( GameController p_gameManager )
	{
	//	_controller.getUI().rem
		Debug.Log ("Exiting In Intro State");
		//_backButton.onClick.RemoveAllListeners ();
	}


	
//------------------- Private Implementation -------------------
//--------------------------------------------------------------
	private Button 			_backButton;
	private PlayFieldView		_introView;
	private GameController 	_gameController;
    private ActiveCustomerSet _customerController;

	private bool _gotoSplash = false;

	private void onBackClick()
	{
		_gotoSplash = true;
	}
}
