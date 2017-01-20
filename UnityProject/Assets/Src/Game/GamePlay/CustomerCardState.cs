
using System.Collections.Generic;
using UnityEngine;


public class CustomerCardState
{
    public int lastPlayerIndex { get; set; }
    public CustomerCardData cardData { get; private set; }

    public static CustomerCardState Create(CustomerCardData cardData)
    {
        CustomerCardState state = new CustomerCardState();
        state.cardData = cardData;
        state.lastPlayerIndex = -1;

        return state;
    }

    public int GetIngredientReqLeft(CardType type)
    {
        int value = 0;
        foreach(IngredientCardData cardData in _ingredientList)
        {
            if (cardData.cardType == type)
                value++;
        }
        return _getIngredientReq(type) - value;
    }
    
    public bool AddIngredient(IngredientCardData card, int playerIndex)
    {
        if (GetIngredientReqLeft(card.cardType) <= 0)
        {
            return false;
        }

        _ingredientList.Add(card);
        lastPlayerIndex = playerIndex;
        return true;
    }



//------------------- Private Implementation -------------------
//--------------------------------------------------------------
    private List<IngredientCardData> _ingredientList = new List<IngredientCardData>();


    private int _getIngredientReq(CardType type)
    {
        switch(type)
        {
            case CardType.Meat:     return cardData.meatRequirement;
            case CardType.Veggie:   return cardData.veggieRequirement;
            case CardType.Topping:  return cardData.toppingRequirement;
        }
        Debug.LogError("Card Type shouldn't request requirements: " + type);
        return 0;
    }
}
