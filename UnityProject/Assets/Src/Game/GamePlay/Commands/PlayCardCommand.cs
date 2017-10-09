using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayCardCommand : ICommand
{
    private CustomerCardState _customer;
    private IngredientCardData _ingredient;
    private PlayerState _playerState;
    private MoveRequest _moveRequest;
    
    private int _savedPlayerIndex = -1;
    private int _savedCardsPlayed = 0;

    public static PlayCardCommand Create(
        MoveRequest moveRequest,
        PlayerState playerState,
        CustomerCardState customer,
        IngredientCardData ingredient)
    {
        PlayCardCommand command = new PlayCardCommand();
        command._customer = customer;
        command._ingredient = ingredient;
        command._playerState = playerState;
        command._moveRequest = moveRequest;
        return command;
    }

    private PlayCardCommand() { }

    public bool isLinked
    {
        get { return false; }
    }

    public void Execute()
    {
        Assert.IsNotNull(_customer);
        Assert.IsNotNull(_ingredient);
        _savedPlayerIndex = _customer.lastPlayerIndex;
        _customer.AddIngredient(_ingredient, _playerState.index);
        _playerState.hand.SetCard(_moveRequest.handSlot, null);
        _savedCardsPlayed = _playerState.cardsPlayed;
        _playerState.cardsPlayed++;
    }

    public void Undo()
    {
        Assert.IsNotNull(_customer);
        Assert.IsNotNull(_ingredient);
        _playerState.cardsPlayed = _savedCardsPlayed;
        _customer.RemoveIngredient(_ingredient, _savedPlayerIndex);
        _playerState.hand.SetCard(_moveRequest.handSlot, _ingredient);
    }
}
