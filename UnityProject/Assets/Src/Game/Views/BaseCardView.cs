using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GhostGen;
using UnityEngine.EventSystems;

[System.Serializable]
public class BaseCardView : UIView
{
    public Image _backgroundImg;
    public Image _cardIcon;
    public Image _cardTypeIcon;
    public Text _titleLbl;
    public Text _foodValueLbl;
    

    protected BaseCardData _cardData = null;

   

    public BaseCardData cardData
    {
        set
        {
            if(_cardData != value)
            {
                _cardData = value;
                invalidateFlag = INVALIDATE_STATIC_DATA;
            }
        }

        get
        {
            return _cardData;
        }
    }
    
    protected override void OnViewUpdate()
    {
        if( _cardData != null && IsInvalid(INVALIDATE_STATIC_DATA) )
        {
            _titleLbl.text = _cardData.titleKey; // TODO: Localize!
            _cardIcon.name = _cardData.iconName;
        }
    }
}
