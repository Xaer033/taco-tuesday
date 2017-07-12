using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMatchState   
{
    public PlayerGroup          playerGroup         { get; private set; }
    public ActiveCustomerSet    activeCustomerSet   { get; private set; }

    public CardDeck     customerDeck    { get; private set; }
    public CardDeck     ingredientDeck  { get; private set; }

    public static GameMatchState Create(
        PlayerGroup playerGroup, 
        ActiveCustomerSet customerGroup,
        CardDeck customerDeck,
        CardDeck indredientDeck)
    {
        GameMatchState matchState = new GameMatchState();
        matchState.playerGroup = playerGroup;
        matchState.activeCustomerSet = customerGroup;
        matchState.customerDeck = customerDeck;
        matchState.ingredientDeck = indredientDeck;
        return matchState;
    }

    public bool isGameOver
    {
        get
        {
            return  customerDeck.isEmpty &&
                    activeCustomerSet.isAllSlotsEmpty;
        }
    }

    private GameMatchState() { }
}
