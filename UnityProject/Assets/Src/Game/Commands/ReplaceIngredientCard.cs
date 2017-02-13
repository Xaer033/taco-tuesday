using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ReplaceIngredientCard : ICommand
{
    private CardDeck _ingredientDeck;
    private PlayerHand _hand;
    private int _handIndex;

    private IngredientCardData _savedCard;

    public static ReplaceIngredientCard Create(
        int handIndex,
        PlayerHand hand,
        CardDeck ingredientDeck)
    {
        ReplaceIngredientCard command = new ReplaceIngredientCard();
        command._handIndex = handIndex;
        command._hand = hand;
        command._ingredientDeck = ingredientDeck;
        return command;
    }

    private ReplaceIngredientCard() { }

    public bool isLinked
    {
        get { return true; }
    }

    public void Execute()
    {
        IngredientCardData newCard = _ingredientDeck.Pop() as IngredientCardData;
        _savedCard = _hand.ReplaceCard(_handIndex, newCard);
    }

    public void Undo()
    {
        IngredientCardData oldCard = _hand.ReplaceCard(_handIndex, _savedCard);
        Assert.IsNotNull(oldCard);
        _ingredientDeck.Push(oldCard);
    }
}
