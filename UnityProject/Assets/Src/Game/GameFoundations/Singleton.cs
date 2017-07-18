using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;


public class Singleton : MonoBehaviour 
{
    public CardResourceBank cardResourceBank;
    public VFXBank          vfxBank;
    public GuiManager       gui;
    public FontManager      fontManager;

    public GameStateMachine     gameStateMachine    { get; private set; }
    public SessionFlags         sessionFlags        { get; private set; }
    public NetworkManager       networkManager      { get; private set; }

    private IStateFactory _stateFactory;

    public void Awake()
    {
        _instance = this;

        _stateFactory = new TacoTuesdayStateFactory();
        gameStateMachine = new GameStateMachine(_stateFactory);

        sessionFlags = new SessionFlags();
        networkManager = gameObject.AddComponent<NetworkManager>();

        Input.multiTouchEnabled = false; //This needs to go elsewere 
    }
    public void Start()
    {
        _postInit();

        gameStateMachine.ChangeState(TacoTuesdayState.INTRO);
    }


    public void Update()
    {
        gameStateMachine.Step(Time.deltaTime);
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
        networkManager.PostInit();
    }
}
