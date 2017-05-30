﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;

[RequireComponent(typeof(FontManager))]
public class Singleton : MonoBehaviour
{
    public Canvas mainCanvas;
    public Camera guiCamera;

    public CardResourceBank cardResourceBank;
    public VFXBank vfxBank;

    //public FontManager fontManager { get; private set; }
    public ViewFactory viewFactory { get; private set; }
    public GameManager gameManager
    {
        get
        {
            return GameManager.instance;
        }
    }

    public GameController gameController
    {
        get
        {
            return gameManager.gameController;
        }
    }

    void Awake()
    {
        _instance = this;
        cardResourceBank.Initialize();
        viewFactory = new ViewFactory(mainCanvas);
    }

    void Update()
    {
        viewFactory.Step();
    }

    private static Singleton _instance = null;
    public static Singleton instance
    {
        get
        {
            return _instance;
        }
    }

    
}
