﻿
using System.Collections.Generic;
using UnityEngine;


public class CustomerCardState
{
    public int lastPlayerIndex { get; set; }
    public int slotIndex { get; set; }

    public CustomerCardData cardData { get; private set; }

    public bool isComplete
    {
        get
        {
            return GetIngredientReqLeft(CardType.MEAT) == 0 &&
                    GetIngredientReqLeft(CardType.VEGGIE) == 0 &&
                    GetIngredientReqLeft(CardType.TOPPING) == 0;
        }
    }

    public int totalScore
    {
        get
        {
            int ingredientScore = 0;
            for(int i = 0; i < _ingredientList.Count; ++i)
            {
                ingredientScore += _ingredientList[i].foodValue;
            }

            int preModifierScore = ingredientScore + cardData.baseReward;
            return CardUtility.ApplyModifier(cardData.modifier, preModifierScore);
        }
    }


    public static CustomerCardState Create(CustomerCardData cardData)
    {
        CustomerCardState state = new CustomerCardState();
        state.cardData = cardData;
        state.lastPlayerIndex = -1;

        return state;
    }

    public int GetIngredientReqLeft(string type)
    {
        int value = 0;
        foreach(IngredientCardData cardData in _ingredientList)
        {
            if (cardData.cardType == type)
                value++;
        }
        return _getIngredientReq(type) - value;
    }
    
    public bool CanAcceptCard(IngredientCardData card)
    {
        return GetIngredientReqLeft(card.cardType) > 0;
    }
    public bool AddIngredient(IngredientCardData card, int playerIndex)
    {
        if (!CanAcceptCard(card))
        {
            return false;
        }

        _ingredientList.Add(card);
        lastPlayerIndex = playerIndex;
        return true;
    }

    public void RemoveIngredient(IngredientCardData card, int playerIndex)
    {   
        _ingredientList.Remove(card);
        lastPlayerIndex = playerIndex;
    }


    //------------------- Private Implementation -------------------
    //--------------------------------------------------------------
    private List<IngredientCardData> _ingredientList = new List<IngredientCardData>();


    private int _getIngredientReq(string type)
    {
        switch(type)
        {
            case CardType.MEAT:     return cardData.meatRequirement;
            case CardType.VEGGIE:   return cardData.veggieRequirement;
            case CardType.TOPPING:  return cardData.toppingRequirement;
        }
        Debug.LogError("Card Type shouldn't request requirements: " + type);
        return 0;
    }
}
