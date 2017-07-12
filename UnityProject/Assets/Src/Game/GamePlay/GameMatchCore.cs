﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameMatchCore 
{
    public GameMatchState   matchState  { get; private set; }

    private CommandFactory _commandFactory = new CommandFactory();
    
    // Broadcast events
    private Action _onPlayOnCustomer;
    private Action<bool> _onResolveScore;
    private Action _onEndTurn;


    public static GameMatchCore Create(List<PlayerState> playerList, int randomSeed = -1)
    {
        GameMatchCore server = new GameMatchCore(playerList, randomSeed);
        return server;
    }

    private GameMatchCore(List<PlayerState> playerList, int randomSeed)
    {
        CardDeck customerDeck = CardDeck.FromFile("Decks/CustomerDeck");
        customerDeck.Shuffle(randomSeed);

        CardDeck ingredientDeck = CardDeck.FromFile("Decks/IngredientDeck");
        ingredientDeck.Shuffle(randomSeed);

        // Also no commands for starting player hands
        ActiveCustomerSet   customerGroup   = _createCustomerCards(customerDeck);
        PlayerGroup         playerGroup     = _createPlayerHands(playerList, ingredientDeck);

        matchState = GameMatchState.Create(playerGroup, customerGroup, customerDeck, ingredientDeck);
    }

    public PlayerGroup playerGroup
    {
        get { return matchState.playerGroup; }
    }

    public ActiveCustomerSet activeCustomerSet
    {
        get { return matchState.activeCustomerSet; }
    }

    public event Action onPlayOnCustomer
    {
        add { _onPlayOnCustomer += value; }
        remove { _onPlayOnCustomer -= value; }
    }

    public event Action<bool> onResolveScore
    {
        add { _onResolveScore += value; }
        remove { _onResolveScore -= value; }
    }

    public event Action onEndTurn
    {
        add { _onEndTurn += value; }
        remove { _onEndTurn -= value; }
    }

    public bool PlayCardOnCustomer(
       int playerIndex,
       int handIndex,
       int customerIndex)
    {
        if (isGameOver)
        {
            Debug.LogError("Game is over!");
            return false;
        }

        if (!activeCustomerSet.IsSlotActive(customerIndex)) { return false; }

        PlayerState playerState = playerGroup.GetPlayer(playerIndex);
        IngredientCardData ingredientData = playerState.hand.GetCard(handIndex);
        CustomerCardState customerState = activeCustomerSet.GetCustomerByIndex(customerIndex);

        if (playerState.cardsPlayed >= PlayerState.kMaxCardsPerTurn) { return false; }
        if (!customerState.CanAcceptCard(ingredientData)) { return false; }

        _playCard(handIndex, playerState, customerState, ingredientData);
        _replaceIngredientCard(handIndex, playerState.hand);
        _playCardEvent();
        return true;
    }

    public bool ResolveCustomerCard(int customerSlotIndex, int playerIndex)
    {
        PlayerState player = playerGroup.GetPlayer(playerIndex);
        Assert.IsNotNull(player);
        CustomerCardState customerState = activeCustomerSet.GetCustomerByIndex(customerSlotIndex);

        bool result = _resolveCustomerCard(customerState, player);
        if (result)
        {
            _createNewCustomer(customerSlotIndex);
        }
        _resolveMoveEvent(result);
        return result;
    }

    public void EndPlayerTurn()
    {
        ICommand command = ChangePlayerTurn.Create(playerGroup);
        _commandFactory.Execute(command);
        _endTurnEvent();
    }

    public bool UndoLastAction()
    {
        return _commandFactory.Undo();
    }

    public bool isGameOver
    {
        get { return matchState.isGameOver; }
    }

    private void _replaceIngredientCard(int handIndex, PlayerHand hand)
    {
        ICommand command = ReplaceIngredientCard.Create(
            handIndex,
            hand,
            matchState.ingredientDeck);
        _commandFactory.Execute(command);
    }

    private bool _resolveCustomerCard(
        CustomerCardState customer,
        PlayerState player)
    {
        if (customer.isComplete)
        {
            ICommand resolve = ResolveScoreCommand.Create(player, customer);
            _commandFactory.Execute(resolve);
            return true;
        }
        return false;
    }

    private void _createNewCustomer(int customerSlotId)
    {
        ICommand command = CreateActiveCustomerCommand.Create(
            playerGroup.activePlayer,
            customerSlotId,
            matchState.customerDeck,
            activeCustomerSet);

        _commandFactory.Execute(command);
    }

    private void _playCard(
        int handSlot,
        PlayerState playerState,
        CustomerCardState customer,
        IngredientCardData ingredient)
    {
        ICommand command = PlayCardCommand.Create(
            handSlot,
            playerState,
            customer,
            ingredient);

        _commandFactory.Execute(command);
    }


    private void _playCardEvent()
    {
        if (_onPlayOnCustomer != null)
        {
            _onPlayOnCustomer();
        }
    }

    private void _resolveMoveEvent(bool result)
    {
        if (_onResolveScore != null)
        {
            _onResolveScore(result);
        }
    }

    private void _endTurnEvent()
    {
        if (_onEndTurn != null)
        {
            _onEndTurn();
        }
    }

    private PlayerGroup _createPlayerHands(List<PlayerState> playerList, CardDeck ingredientDeck)
    {
        PlayerGroup group =PlayerGroup.Create(playerList);
        for (int i = 0; i < group.playerCount; ++i)
        {
            for (int j = 0; j < PlayerState.kHandSize; ++j)
            {
                IngredientCardData cardData = ingredientDeck.Pop() as IngredientCardData;
                PlayerState playerState = group.GetPlayer(i);
                playerState.hand.SetCard(j, cardData);
            }
        }
        return group;
    }

    private ActiveCustomerSet _createCustomerCards(CardDeck customerDeck)
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
        return customers;
    }

}