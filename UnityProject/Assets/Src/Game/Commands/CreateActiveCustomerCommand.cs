using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateActiveCustomerCommand : ICommand
{
    private PlayerState _activePlayer;
    private CardDeck _customerDeck;
    private ActiveCustomerSet _customerSet;
    private int _slotIndex;

    private CustomerCardState _savedCustomerState;

    public static CreateActiveCustomerCommand Create(
        PlayerState activePlayer,
        int slotIndex,
        CardDeck customerDeck,
        ActiveCustomerSet customerSet
        )
    {
        CreateActiveCustomerCommand command = new CreateActiveCustomerCommand();
        command._activePlayer   = activePlayer;
        command._customerDeck   = customerDeck;
        command._slotIndex      = slotIndex;
        command._customerSet    = customerSet;
        return command;
    }

    private CreateActiveCustomerCommand()
    {
    }

    public bool isLinked
    {
        get { return true; }
    }

    public void Execute()
    {
        _savedCustomerState = _customerSet.GetCustomerByIndex(_slotIndex);
        _activePlayer.deadCustomerStack.Push(_savedCustomerState.cardData);

        CustomerCardData card = _customerDeck.Pop() as CustomerCardData;
        if (card != null)
        {
            CustomerCardState newState = CustomerCardState.Create(card);
            _customerSet.SetCustomerAtIndex(_slotIndex, newState);
        }
        else
        {
            _customerSet.SetCustomerAtIndex(_slotIndex, null);
        }
    }

    public void Undo()
    {
        _activePlayer.deadCustomerStack.Pop();
        CustomerCardState undoState = _customerSet.GetCustomerByIndex(_slotIndex);
        if (undoState != null)
        {
            _customerDeck.Push(undoState.cardData);
        }
        _customerSet.SetCustomerAtIndex(_slotIndex, _savedCustomerState);
    }
}
