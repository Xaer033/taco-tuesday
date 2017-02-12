using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameLogic
{
    public PlayerGroup playerGroup { get; private set; }
    public ActiveCustomerSet activeCustomerSet { get; private set; }

    private CardDeck _customerDeck;
    private CardDeck _ingredientDeck;

    private CommandFactory _commandFactory = new CommandFactory();


    public static GameLogic Create(List<PlayerState> playerList)
    {
        return new GameLogic(playerList);
    }

    private GameLogic(List<PlayerState> playerList)
    {
        _customerDeck = CardDeck.FromFile("Decks/CustomerDeck");
        _customerDeck.Shuffle();

        _ingredientDeck = CardDeck.FromFile("Decks/IngredientDeck");
        _ingredientDeck.Shuffle();

        activeCustomerSet = ActiveCustomerSet.Create();

        // Intentionally not using commands here, as we don't want to be able to 
        // undo the first set of customers
        for (int i = 0; i < ActiveCustomerSet.kMaxActiveCustomers; ++i)
        {
            CustomerCardData cardData = _customerDeck.Pop() as CustomerCardData;
            CustomerCardState cardState = CustomerCardState.Create(cardData);
            activeCustomerSet.SetCustomerAtSlot(i, cardState);
        }

        // Also no commands for starting player hands
        playerGroup = PlayerGroup.Create(playerList);
        _setupPlayerHands();
    }

    private void _setupPlayerHands()
    {
        Assert.IsNotNull(playerGroup);

        for (int i = 0; i < playerGroup.playerCount; ++i)
        {
            for (int j = 0; j < PlayerState.kHandSize; ++j)
            {
                IngredientCardData cardData = _customerDeck.Pop() as IngredientCardData;
                playerGroup.GetPlayer(i).hand.SetCard(j, cardData);
            }
        }
    }
    
    public bool PlayCardOnCustomer(
        IngredientCardData ingredientData, 
        int playerIndex, 
        int customerSlotId)
    {
        if (!activeCustomerSet.IsSlotActive(customerSlotId)) { return false; }

        CustomerCardState customerState = activeCustomerSet.GetCustomerAtSlot(customerSlotId);
        if(!customerState.CanAcceptCard(ingredientData)) { return false; }
        
        _playCard(customerState, ingredientData, playerIndex);

        PlayerState player = playerGroup.GetPlayer(playerIndex);// playerList[playerIndex];
        Assert.IsNotNull(player);
        if (_resolveCustomerCard(customerState, player))
        {
            _createNewCustomer(customerSlotId);
        }

        return true;
    }

    public void EndPlayerTurn()
    {
        ChangePlayerTurn command = ChangePlayerTurn.Create(playerGroup);
        _commandFactory.Execute(command);
    }

    public bool UndoLastAction()
    {
        return _commandFactory.Undo();
    }

    
    private bool _resolveCustomerCard(
        CustomerCardState customer,
        PlayerState player)
    {
        if(customer.isComplete)
        {
            ResolveScoreCommand resolve = ResolveScoreCommand.Create(player, customer);
            _commandFactory.Execute(resolve);
            return true;
        }
        return false;
    }

    private void _createNewCustomer(int customerSlotId)
    {
        CreateActiveCustomerCommand command = CreateActiveCustomerCommand.Create(
            customerSlotId,
            _customerDeck,
            activeCustomerSet);
        _commandFactory.Execute(command);
    }

    private void _playCard(
        CustomerCardState customer,
        IngredientCardData ingredient,
        int playerIndex)
    {
        PlayCardCommand command = PlayCardCommand.Create(
            customer,
            ingredient,
            playerIndex);

        _commandFactory.Execute(command);
    }
}
