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

        _introView = _controller.GetUI().CreateView(TacoTuesdayViews.IntroMovie, 0);
        _introView.OnIntroTransitionEvent += _introView_OnIntroTransitionEvent;
        //_backButton = GameObject.Find ("backButton").GetComponent< Button > ();
        //_backButton.onClick.AddListener( onBackClick );

        TextAsset deckJson = Resources.Load<TextAsset>("Deck/DefaultDeck");
        CardDeck deck = CardDeck.FromJson(deckJson.text);

        for (int i = 0; i < deck.cardList.Count; ++i)
        {
            GameManager.cardResourceBank.CreateCardView(deck.cardList[i], _introView.transform);
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
	private UIView 			_introView;
	private GameController 	_controller;

	private bool _gotoSplash = false;

	private void onBackClick()
	{
		_gotoSplash = true;
	}
}
