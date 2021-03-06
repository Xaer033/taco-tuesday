﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GhostGen;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class BaseCardView : UIView
{
    public Image _backgroundImg;
    public Image _cardIcon;
    public Image _cardTypeIcon;
    public TextMeshProUGUI _titleLbl;
    public TextMeshProUGUI _foodValueLbl;
    
    protected BaseCardData _cardData = null;

    public BaseCardData cardData
    {
        set
        {
            if(_cardData != value)
            {
                _cardData = value;
                invalidateFlag |= InvalidationFlag.STATIC_DATA;
            }
        }

        get
        {
            return _cardData;
        }
    }
    
    protected override void OnViewUpdate()
    {
        if( _cardData != null && IsInvalid(InvalidationFlag.STATIC_DATA) )
        {
            _titleLbl.text = _cardData.titleKey; // TODO: Localize!
            _cardIcon.name = _cardData.iconName;
        }
    }
}
