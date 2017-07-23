using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ReplaceIngredientCards : ICommand
{
    private CardDeck    _ingredientDeck;
    private PlayerHand  _hand;

    // Depricated
    private int _handIndex;
    private IngredientCardData _savedCard;

    private List<int>                   _savedIndices   = new List<int>(PlayerHand.kDefaultHandSize);
    private List<IngredientCardData>    _savedCards     = new List<IngredientCardData>(PlayerHand.kDefaultHandSize);

    public static ReplaceIngredientCards Create(
        PlayerHand hand,
        CardDeck ingredientDeck)
    {
        ReplaceIngredientCards command = new ReplaceIngredientCards();
        command._hand = hand;
        command._ingredientDeck = ingredientDeck;
        return command;
    }

    private ReplaceIngredientCards() { }

    public bool isLinked
    {
        get { return false; }
    }

    public void Execute()
    {
        _savedCards.Clear();
        _savedIndices.Clear();

        int count = PlayerHand.kDefaultHandSize;
        for(int i = 0; i < count; ++i)
        {
            IngredientCardData cardData = _hand.GetCard(i);
            if(cardData == null)
            {
                IngredientCardData newCard = _ingredientDeck.Pop() as IngredientCardData;
                IngredientCardData oldData = _hand.ReplaceCard(i, newCard);

                _savedCards.Add(oldData);
                _savedIndices.Add(i);
            }
        }
    }

    public void Undo()
    {
        int count = _savedCards.Count;
        for (int i = count - 1; i >= 0; --i) // done in reverse to maintain the proper deck order
        {
            int index = _savedIndices[i];
            IngredientCardData savedCard = _savedCards[i];
            IngredientCardData oldCard = _hand.ReplaceCard(index, savedCard);

            Assert.IsNotNull(oldCard);
            _ingredientDeck.Push(oldCard);
        }
    }
}
