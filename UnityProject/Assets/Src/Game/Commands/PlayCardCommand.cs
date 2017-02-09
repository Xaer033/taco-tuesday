using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayCardCommand : ICommand
{
    private CustomerCardState _customer;
    private IngredientCardData _ingredient;

    private int _playerIndex = -1;
    private int _savedPlayerIndex = -1;

    public static PlayCardCommand Create(
        CustomerCardState customer,
        IngredientCardData ingredient,
        int playerIndex)
    {
        PlayCardCommand command = new PlayCardCommand();
        command._customer = customer;
        command._ingredient = ingredient;
        command._playerIndex = playerIndex;
        return command;
    }

    private PlayCardCommand() { }

    public void Execute()
    {
        Assert.IsNotNull(_customer);
        Assert.IsNotNull(_ingredient);
        _savedPlayerIndex = _customer.lastPlayerIndex;
        _customer.AddIngredient(_ingredient, _playerIndex);
    }

    public void Undo()
    {
        Assert.IsNotNull(_customer);
        Assert.IsNotNull(_ingredient);
        _customer.RemoveIngredient(_ingredient, _savedPlayerIndex);
    }
}
