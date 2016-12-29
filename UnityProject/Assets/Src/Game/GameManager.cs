using UnityEngine;
using System.Collections;
using GhostGen;

public class GameManager : MonoBehaviour
{
	
	public GameController gameController;
    public Canvas guiCanvas { get; private set; }

    private IViewFactory _viewFactory;
    private IStateFactory _stateFactory;

	void Awake()
	{
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
