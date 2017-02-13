using System.Collections;
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

    //public GameManager gameManager { get; private set; }
    //public FontManager fontManager { get; private set; }
    public ViewFactory viewFactory { get; private set; }


    void Awake()
    {
        viewFactory = new ViewFactory(mainCanvas);
        _instance = this;
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
