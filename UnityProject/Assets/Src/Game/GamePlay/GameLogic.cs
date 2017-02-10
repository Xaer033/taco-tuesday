using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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

    private CommandFactory _commandFactory = new CommandFactory();


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

    
    public bool PlayCardOnCustomer(
        IngredientCardData ingredientData, 
        int playerIndex, 
        int customerSlotId)
    {
        if (!activeCustomerSet.IsSlotActive(customerSlotId)) { return false; }

        CustomerCardState customerState = activeCustomerSet.GetCustomerAtSlot(customerSlotId);
        if(!customerState.CanAcceptCard(ingredientData)) { return false; }
        PlayerState player = playerList[playerIndex];
        Assert.IsNotNull(player);

        PlayCardCommand command = PlayCardCommand.Create(
            customerState, 
            ingredientData, 
            playerIndex);

        _commandFactory.Execute(command);

        if(_resolveCustomerCard(customerState, player))
        {
            _createNewCustomer(customerSlotId);
        }

        return true;
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
}
