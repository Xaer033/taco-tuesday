﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GhostGen;

[System.Serializable]
public class CardView : UIView
{
    protected const string INVALIDATE_STATIC_DATA   = "invalidate_static_data";
    protected const string INVALIDATE_DYNAMIC_DATA  = "invalidate_dynamic_data";

    public Image _backgroundImg;
    public Image _cardIcon;
    public Image _cardTypeIcon;
    public Text _titleLbl;
    public Text _foodValueLbl;

    public GameObject _meatReqObj;
    public GameObject _veggieReqObj;
    public GameObject _toppingReqObj;

    private Text _meatReqLbl;
    private Text _veggieReqLbl;
    private Text _toppingReqLbl;

    private BaseCardData _cardData = null;
    private CustomerCardState _cardState = null;

    private void Awake()
    {
        if(_meatReqObj != null)
            _meatReqLbl     = _meatReqObj.GetComponentInChildren<Text>();

        if (_veggieReqObj != null)
            _veggieReqLbl   = _veggieReqObj.GetComponentInChildren<Text>();

        if (_toppingReqObj != null)
            _toppingReqLbl  = _toppingReqObj.GetComponentInChildren<Text>();
    }

    public BaseCardData cardData
    {
        set
        {
            if(_cardData != value)
            {
                _cardData = value;
                InvalidateFlag = INVALIDATE_STATIC_DATA;
            }
        }
    }

    public CustomerCardState cardState
    {
        set
        {
            if(_cardState != value)
            {
                _cardState = value;
                InvalidateFlag = INVALIDATE_DYNAMIC_DATA;
            }
        }
    }

    protected override void OnUpdateView(string invalidateFlag)
    {
        if( _cardData != null && 
            (invalidateFlag == UIView.INVALIDATE_ALL ||
            invalidateFlag == INVALIDATE_STATIC_DATA) )
        {
            _titleLbl.text = _cardData.titleKey; // TODO: Localize!
            _cardIcon.name = _cardData.iconName;
            
            if(_cardData.cardType == CardType.Customer)
            {
                _setCustomerCard((CustomerCardData)_cardData);
            }
            else
            {
                _setIngredientCard((IngredientCardData)_cardData);
            }
        }

        if( _cardState != null &&
            (invalidateFlag == UIView.INVALIDATE_ALL ||
            invalidateFlag == INVALIDATE_DYNAMIC_DATA) )
        {
            _meatReqLbl.text = string.Format("x{0}", _cardState.GetIngredientReqLeft(CardType.Meat));
            _veggieReqLbl.text = string.Format("x{0}", _cardState.GetIngredientReqLeft(CardType.Veggie));
            _toppingReqLbl.text = string.Format("x{0}", _cardState.GetIngredientReqLeft(CardType.Topping));
        }
    }

    private void _setIngredientCard(IngredientCardData ingredientData)
    {
        CardResourceBank cardBank = GameManager.cardResourceBank;
        _backgroundImg.sprite = cardBank.GetIngredientBG(ingredientData.cardType);
        _cardTypeIcon.sprite = cardBank.GetIngredientTypeIcon(ingredientData.cardType);
        _foodValueLbl.text = string.Format("{0}", ingredientData.foodValue);
        _cardIcon.sprite = cardBank.GetMainIcon(ingredientData.iconName);
    }

    private void _setCustomerCard(CustomerCardData customerData)
    {
        if(customerData.meatRequirement == 0)
        {
            _meatReqObj.SetActive(false);
        }
        else
        {
            _meatReqLbl.text = string.Format("x{0}", customerData.meatRequirement);
        }

        if(customerData.veggieRequirement == 0)
        {
            _veggieReqObj.SetActive(false);
        }
        else
        {
            _veggieReqLbl.text = string.Format("x{0}", customerData.veggieRequirement);
        }

        if(customerData.toppingRequirement == 0)
        {
            _toppingReqObj.SetActive(false);
        }
        else
        {
            _toppingReqLbl.text = string.Format("x{0}", customerData.toppingRequirement);
        }

        _foodValueLbl.text = string.Format("{0}", customerData.baseReward);
    }
}
