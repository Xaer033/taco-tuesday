﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public enum CardType
{
    None = -1,
    Meat,
    Veggie,
    Topping,
    Customer
}


public class CardTypeUtility
{
    public static CardType FromString(string cardType)
    {
        switch(cardType.ToLower())
        {
            case "meat": return CardType.Meat;
            case "veggie": return CardType.Veggie;
            case "topping": return CardType.Topping;
            case "customer": return CardType.Customer;
        }

        Debug.LogError(string.Format("We don't handle card type: {0}!", cardType));
        return CardType.None;
    }
}

public class BaseCardData
{
    public string      name;
    public string      iconName;
    public CardType    cardType;
}

public class CustomerCardData : BaseCardData
{
    public int      meatRequirement;     
    public int      veggieRequirement;   
    public int      toppingRequirement;  
    public int      baseReward;          
}

public class IngredientCardData : BaseCardData
{
    public int      foodValue;
}


public class CardDataFactory
{
    public static BaseCardData CreateFromJson(JToken cardToken)
    {
        CardType type = CardTypeUtility.FromString(cardToken.Value<string>("cardType"));
        string name = cardToken.Value<string>("name");
        string iconName = cardToken.Value<string>("iconName");

        if (type == CardType.Customer)
        {
            CustomerCardData cData = new CustomerCardData();
            cData.cardType = type;
            cData.name = name;
            cData.iconName = iconName;
            cData.baseReward = cardToken.Value<int>("baseReward");
            cData.meatRequirement = cardToken.Value<int>("meatRequirement");
            cData.veggieRequirement = cardToken.Value<int>("veggieRequirement");
            cData.toppingRequirement = cardToken.Value<int>("toppingRequirement");
            return cData;
        }
        else
        {
            IngredientCardData iData = new IngredientCardData();
            iData.cardType = type;
            iData.name = name;
            iData.iconName = iconName;
            iData.foodValue = cardToken.Value<int>("foodValue");
            return iData;
        }
    }
}