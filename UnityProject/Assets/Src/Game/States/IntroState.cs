using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GhostGen;

public class IntroState : IGameState
{
	public void Init( GameController p_gameManager )
	{
		Debug.Log ("Entering In Intro State");
		_controller = p_gameManager;

        _introView = _controller.GetUI().CreateView(TacoTuesdayViews.IntroMovie, 0) as IntroView;
        _introView.OnIntroTransitionEvent += _introView_OnIntroTransitionEvent;
        //_backButton = GameObject.Find ("backButton").GetComponent< Button > ();
        //_backButton.onClick.AddListener( onBackClick );

        CardDeck customerDeck = CardDeck.FromFile("Decks/CustomerDeck");
        customerDeck.Shuffle();

        for (int i = 0; i < customerDeck.cardList.Count; ++i)
        {
            GameManager.cardResourceBank.CreateCardView(customerDeck.cardList[i], _introView.cardParent);
        }


        CardDeck ingredientDeck = CardDeck.FromFile("Decks/IngredientDeck");
        ingredientDeck.Shuffle();

        for (int i = 0; i < ingredientDeck.cardList.Count; ++i)
        {
            GameManager.cardResourceBank.CreateCardView(ingredientDeck.cardList[i], _introView.cardParent);
        }
    }

    private void _introView_OnIntroTransitionEvent(UIView p_view)
    {
        Debug.Log(string.Format("View {0} has fininished intro", p_view.name));
    }

    public void Step( float p_deltaTime )
	{
		if (_gotoSplash) 
		{
			_controller.ChangeState (TacoTuesdayState.Intro);
			_gotoSplash = false;
		}

        //Transform camTransform = Camera.main.transform;
        //Vector3 lookPos = camTransform.position + camTransform.forward * 10.0f;
        //Camera.main.transform.LookAt(lookPos, -Input.gyro.gravity.normalized);
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
	private IntroView		_introView;
	private GameController 	_controller;

	private bool _gotoSplash = false;

	private void onBackClick()
	{
		_gotoSplash = true;
	}
}
