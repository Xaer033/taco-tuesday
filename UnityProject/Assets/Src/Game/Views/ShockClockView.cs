using System;
using UnityEngine;
using GhostGen;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ShockClockView : UIView 
{
    public TextMeshProUGUI timerLabel;

    private string _timerText;
    
    public string timerText
    {
        set
        {
            if(_timerText != value)
            {
                _timerText = value;
                invalidateFlag = InvalidationFlag.DYNAMIC_DATA;
            }
        }
    }
    
    protected override void OnViewUpdate()
    {
        base.OnViewUpdate();

        if(IsInvalid(InvalidationFlag.DYNAMIC_DATA))
        {
            timerLabel.text = _timerText;
        }
    }
}
