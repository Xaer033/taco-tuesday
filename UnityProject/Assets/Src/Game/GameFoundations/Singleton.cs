using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;


public class Singleton : MonoBehaviour 
{
    public CardResourceBank cardResourceBank;
    public VFXBank vfxBank;
    public GuiManager gui;
    public FontManager fontManager;


    public GameController   gameController  { get; private set; }
    public SessionFlags     sessionFlags    { get; private set; }

    private IStateFactory _stateFactory;

    public void Awake()
    {
        _instance = this;

        _stateFactory = new TacoTuesdayStateFactory();
        gameController = new GameController(_stateFactory);

        sessionFlags = new SessionFlags();

        Input.multiTouchEnabled = false; //This needs to go elsewere 
    }
    public void Start()
    {
        _postInit();

        gameController.ChangeState(TacoTuesdayState.INTRO);
    }


    public void Update()
    {
        gameController.Step(Time.deltaTime);
        gui.Step(Time.deltaTime);
    }

    private static Singleton _instance = null;
    public static Singleton instance
    {
        get
        {
            return _instance;
        }
    }


    private void _postInit()
    {
        cardResourceBank.PostInit();
        gui.PostInit();
        fontManager.PostInit();
    }
}
