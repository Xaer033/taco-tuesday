using System;
using UnityEngine;
using GhostGen;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class PassPlaySetupView : UIView 
{
    public PlayerInputSetup[] playerInputSetups;
    public Button startButton;
    public Button cancelButton;

    void Awake()
    {
        
    }

    
    protected override void OnViewUpdate()
    {
        base.OnViewUpdate();

    }
}
