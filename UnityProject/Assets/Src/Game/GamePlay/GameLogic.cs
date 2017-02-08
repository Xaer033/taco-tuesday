using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic
{
    public int turnIndex { get; private set; }

    public List<PlayerState> playerList { get; private set; }
    public ActiveCustomerSet activeCustomerSet { get; private set; }

    public PlayerState currentPlayer
    {
        get { return playerList[turnIndex]; }
    }


    private CardDeck _customerDeck;
    private CardDeck _ingredientDeck;


    public static GameLogic Create(List<PlayerState> playerList)
    {
        GameLogic game = new GameLogic();
        game.playerList = playerList;

        game._customerDeck = CardDeck.FromFile("Decks/CustomerDeck");
        game._customerDeck.Shuffle();

        game._ingredientDeck = CardDeck.FromFile("Decks/IngredientDeck");
        game._ingredientDeck.Shuffle();
        return game;
    }

    
    public bool PlayCardOnCustomer(IngredientCardData ingredientData, int playerIndex, int customerSlotId)
    {
        if (!activeCustomerSet.IsSlotActive(customerSlotId))
        {
            return false;
        }

        CustomerCardState cardState = activeCustomerSet.GetCustomerAtSlot(customerSlotId);
        return cardState.AddIngredient(ingredientData, playerIndex);
    }

    public void ResolveCustomerCards()
    {
        for(int i = 0; i < ActiveCustomerSet.kMaxActiveCustomers; ++i)
        {
            CustomerCardState customerState = activeCustomerSet.GetCustomerAtSlot(i);
            if(customerState.isComplete)
            {
                playerList[customerState.lastPlayerIndex].points += customerState.totalScore;

                CustomerCardData card = _customerDeck.Pop() as CustomerCardData;
                CustomerCardState state = CustomerCardState.Create(card);
                activeCustomerSet.SetCustomerAtSlot(i, state);
            }
        }
    }
}
