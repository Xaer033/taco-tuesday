using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CardType
{
    None = 0,
    Meat,
    Vegetable,
    Topping,
    Customer
}



public interface ICardData
{
    string      name        { get; set; }
    string      iconName    { get; set; }
    CardType    cardType    { get; set; }
}

public struct DishCardData : ICardData
{
    public string   name            { get; set; }
    public string   iconName        { get; set; }
    public CardType cardType        { get; set; }

    public int      meatRequirement { get; set; }
    public int      vegeRequirement { get; set; }
    public int      baseReward      { get; set; }
}

public struct IngredientCardData : ICardData
{
    public string   name        { get; set; }
    public string   iconName    { get; set; }
    public CardType cardType    { get; set; }

    public int      foodValue   { get; set; }
}
