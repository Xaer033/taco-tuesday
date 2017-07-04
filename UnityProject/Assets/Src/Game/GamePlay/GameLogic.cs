using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameLogic
{
    public const int kMaxPlayers = 4;

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
            activeCustomerSet.SetCustomerAtIndex(i, cardState);
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
                IngredientCardData cardData = _ingredientDeck.Pop() as IngredientCardData;
                PlayerState playerState = playerGroup.GetPlayer(i);
                playerState.hand.SetCard(j, cardData);
            }
        }
    }
    
    public bool PlayCardOnCustomer(
        int playerIndex,
        int handIndex,
        int customerIndex)
    {
        if(isGameOver)
        {
            Debug.LogError("Game is over!");
            return false;
        }

        if (!activeCustomerSet.IsSlotActive(customerIndex)) { return false; }

        PlayerState         playerState     = playerGroup.GetPlayer(playerIndex);
        IngredientCardData  ingredientData  = playerState.hand.GetCard(handIndex);
        CustomerCardState   customerState   = activeCustomerSet.GetCustomerByIndex(customerIndex);

        if (playerState.cardsPlayed >= PlayerState.kMaxCardsPerTurn) { return false; }
        if (!customerState.CanAcceptCard(ingredientData)) { return false; }
        
        _playCard(handIndex, playerState, customerState, ingredientData);
        _replaceIngredientCard(handIndex, playerState.hand);
        return true;
    }

    public bool ResolveCustomerCard(int customerSlotIndex, int playerIndex)
    {
        PlayerState player = playerGroup.GetPlayer(playerIndex);
        Assert.IsNotNull(player);
        CustomerCardState customerState = activeCustomerSet.GetCustomerByIndex(customerSlotIndex);

        bool result = _resolveCustomerCard(customerState, player);
        if(result)
        {
            _createNewCustomer(customerSlotIndex);
        }
        return result;
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

    private void _replaceIngredientCard(int handIndex, PlayerHand hand)
    {
        ReplaceIngredientCard command = ReplaceIngredientCard.Create(
            handIndex, 
            hand, 
            _ingredientDeck);
        _commandFactory.Execute(command);
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
            playerGroup.activePlayer,
            customerSlotId,
            _customerDeck,
            activeCustomerSet);

        _commandFactory.Execute(command);
    }

    private void _playCard(
        int handSlot,
        PlayerState playerState,
        CustomerCardState customer,
        IngredientCardData ingredient)
    {
        PlayCardCommand command = PlayCardCommand.Create(
            handSlot,
            playerState,
            customer,
            ingredient);

        _commandFactory.Execute(command);
    }

    public bool isGameOver
    {
        get
        {
            return _customerDeck.isEmpty && 
                activeCustomerSet.isAllSlotsEmpty;
        }
    }
}
