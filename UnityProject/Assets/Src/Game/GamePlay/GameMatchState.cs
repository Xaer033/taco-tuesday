using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMatchState   
{
    public PlayerGroup          playerGroup         { get; private set; }
    public ActiveCustomerSet    activeCustomerSet   { get; private set; }

    public CardDeck     customerDeck    { get; private set; }
    public CardDeck     ingredientDeck  { get; private set; }


    private Dictionary<string, BaseCardData> _cardDataMap;

    public static GameMatchState Create(
        List<PlayerState> playerList,
        CardDeck customerDeck,
        CardDeck indredientDeck)
    {
        GameMatchState matchState = new GameMatchState();
        matchState.customerDeck = customerDeck;
        matchState.ingredientDeck = indredientDeck;

        matchState._setupCardMap();
        matchState._setupPlayerHands(playerList, matchState.ingredientDeck);
        matchState._setupCustomerCards(matchState.customerDeck);

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

    public BaseCardData GetCardById(string cardId)
    {
        if(_cardDataMap != null)
        {
            return _cardDataMap[cardId];
        }
        return null;
    }

    private GameMatchState() { }

    private void _setupCardMap()
    {
        _cardDataMap = new Dictionary<string, BaseCardData>();
        
        for(int i = 0; i < customerDeck.cardList.Count; ++i)
        {
            BaseCardData data = customerDeck.cardList[i];
            if(data != null)
            {
                Debug.LogWarning("CustomerCards: " + data.id);
                _cardDataMap[data.id] = data;
            }
        }

        for (int i = 0; i < ingredientDeck.cardList.Count; ++i)
        {
            BaseCardData data = ingredientDeck.cardList[i];
            if (data != null)
            {
                Debug.LogWarning("IngredientCards: " + data.id);
                _cardDataMap[data.id] = data;
            }
        }

        Debug.LogError("Card Map Count: " + _cardDataMap.Count);
    }
    private void _setupPlayerHands(List<PlayerState> playerList, CardDeck ingredientDeck)
    {
        PlayerGroup group = PlayerGroup.Create(playerList);
        for (int i = 0; i < group.playerCount; ++i)
        {
            for (int j = 0; j < PlayerState.kHandSize; ++j)
            {
                IngredientCardData cardData = ingredientDeck.Pop() as IngredientCardData;
                PlayerState playerState = group.GetPlayerByIndex(i);
                playerState.hand.SetCard(j, cardData);
            }
        }
        playerGroup = group;
    }

    private void _setupCustomerCards(CardDeck customerDeck)
    {
        ActiveCustomerSet customers = ActiveCustomerSet.Create();
        // Intentionally not using commands here, as we don't want to be able to 
        // undo the first set of customers
        for (int i = 0; i < ActiveCustomerSet.kMaxActiveCustomers; ++i)
        {
            CustomerCardData cardData = customerDeck.Pop() as CustomerCardData;
            CustomerCardState cardState = CustomerCardState.Create(cardData);
            customers.SetCustomerAtIndex(i, cardState);
        }
        activeCustomerSet = customers;
    }
}
