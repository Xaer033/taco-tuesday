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

    //private IViewFactory _viewFactory;
    private IStateFactory _stateFactory;

    private ViewFactory _viewFactory;

    public ViewFactory viewFactory
    {
        get { return _viewFactory; }
        private set { _viewFactory = value; }
    }

    public static GameManager Get()
    {
        Debug.Assert(_instance != null, "Game Manager not initialized yet, race condition detected!");
        return _instance;
    }

    private static GameManager _instance;

	void Awake()
	{
        _instance = this;
        cardResourceBank = _cardResourceBank;
        cardResourceBank.Initialize();

        fontManager = gameObject.AddComponent<FontManager>();

        guiCanvas = GetComponentInChildren<Canvas>();

        _viewFactory    = new ViewFactory(guiCanvas);
		_stateFactory 	= new TacoTuesdayStateFactory ();

		gameController  = new GameController( _stateFactory, null, guiCanvas );
		gameController.ChangeState( TacoTuesdayState.Intro );


	}
		
		// Update is called once per frame
	void Update () 
	{
		gameController.Step(Time.deltaTime);
        viewFactory.Step();
	}
}
