using UnityEngine;
using System.Collections;
using GhostGen;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{

    public FontManager       fontManager { get; private set; }
    
    public GameController gameController { get; private set; }
    public Canvas guiCanvas { get; private set; }

    public CardResourceBank _cardResourceBank;
    
    private IStateFactory _stateFactory;
   
     
    public static  GameManager instance
    {
        get
        {
            Debug.Assert(_instance != null, "Game Manager not initialized yet, race condition detected!");
            return _instance;        
        }
    }

    private static GameManager _instance;

	void Awake()
	{
        _instance = this;
        fontManager = gameObject.AddComponent<FontManager>();

        guiCanvas = GetComponentInChildren<Canvas>();
        
		_stateFactory 	= new TacoTuesdayStateFactory ();

		gameController  = new GameController( _stateFactory, null, guiCanvas );
		gameController.ChangeState( TacoTuesdayState.INTRO );

        Input.multiTouchEnabled = false;
	}
		
	void Update () 
	{
		gameController.Step(Time.deltaTime);
	}
}
