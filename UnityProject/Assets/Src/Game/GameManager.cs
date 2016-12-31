using UnityEngine;
using System.Collections;
using GhostGen;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{

    public static FontManager       fontManager { get; private set; }
    
    public static CardResourceBank  cardResourceBank { get; private set; }

    public GameController gameController { get; private set; }
    public Canvas guiCanvas { get; private set; }

    public CardResourceBank _cardResourceBank;

    private IViewFactory _viewFactory;
    private IStateFactory _stateFactory;


	void Awake()
	{
        cardResourceBank = _cardResourceBank;
        cardResourceBank.Initialize();

        fontManager = gameObject.AddComponent<FontManager>();

        guiCanvas = GetComponentInChildren<Canvas>();

		_viewFactory 	= new TacoTuesdayViewFactory ();
		_stateFactory 	= new TacoTuesdayStateFactory ();

		gameController  = new GameController( _stateFactory, _viewFactory, guiCanvas );
		gameController.ChangeState( TacoTuesdayState.Intro );


	}
		
		// Update is called once per frame
	void Update () 
	{
		gameController.Step(Time.deltaTime);
	}
}
