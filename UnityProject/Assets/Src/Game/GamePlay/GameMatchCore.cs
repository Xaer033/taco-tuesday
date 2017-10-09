using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameMatchCore 
{
    public GameMatchState   matchState  { get; private set; }

    private CommandFactory  _commandFactory = new CommandFactory();
    
    // Broadcast events
    private Action _onPlayOnCustomer;
    private Action<bool> _onResolveScore;
    private Action _onEndTurn;

    
    public static GameMatchCore Create(
        List<PlayerState> playerList,
        CardDeck customerDeck, 
        CardDeck ingredientDeck)
    {
        GameMatchCore core = new GameMatchCore();

        // Also no commands for starting player hands
        core.matchState = GameMatchState.Create(
            playerList, 
            customerDeck, 
            ingredientDeck);

        return core;
    }

    private GameMatchCore() { }

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

    public bool PlayCardOnCustomer(MoveRequest move)
    {
        if (isGameOver)
        {
            Debug.LogError("Game is over!");
            return false;
        }

        if (!activeCustomerSet.IsSlotActive(move.customerSlot)) { return false; }

        PlayerState playerState = playerGroup.GetPlayerByIndex(move.playerIndex);
        IngredientCardData ingredientData = playerState.hand.GetCard(move.handSlot);
        CustomerCardState customerState = activeCustomerSet.GetCustomerByIndex(move.customerSlot);

        if (playerState.cardsPlayed >= PlayerState.kMaxCardsPerTurn) { return false; }
        if (!customerState.CanAcceptCard(ingredientData)) { return false; }

        _playCard(move, playerState, customerState, ingredientData);
        
        _playCardEvent();
        
        return true;
    }

    public bool ResolveCustomerCard(int customerSlotIndex, int playerIndex)
    {
        PlayerState player = playerGroup.GetPlayerByIndex(playerIndex);
        Assert.IsNotNull(player);
        CustomerCardState customerState = activeCustomerSet.GetCustomerByIndex(customerSlotIndex);

        bool result = _resolveCustomerCards(customerState, player);
        if (result)
        {
            _createNewCustomer(customerSlotIndex);
        }
        _resolveMoveEvent(result);
        return result;
    }

    public void EndPlayerTurn()
    {
        int playerIndex = playerGroup.activePlayer.index;
        PlayerState playerState = playerGroup.GetPlayerByIndex(playerIndex);
        _replaceIngredientCards(playerState.hand);

        ICommand command = ChangePlayerTurn.Create(playerGroup);
        _commandFactory.Execute(command);
        _endTurnEvent();
    }

    public void ClearCommandBuffer()
    {
        _commandFactory.Clear();
    }
    public bool UndoLastAction()
    {
        return _commandFactory.Undo();
    }

    public bool isGameOver
    {
        get { return matchState.isGameOver; }
    }

    private void _replaceIngredientCards(PlayerHand hand)
    {
        ICommand command = ReplaceIngredientCards.Create(
            hand,
            matchState.ingredientDeck);
        _commandFactory.Execute(command);
    }

    private bool _resolveCustomerCards(
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
        MoveRequest moveRequest,
        PlayerState playerState,
        CustomerCardState customer,
        IngredientCardData ingredient)
    {
        ICommand command = PlayCardCommand.Create(
            moveRequest,
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
}
