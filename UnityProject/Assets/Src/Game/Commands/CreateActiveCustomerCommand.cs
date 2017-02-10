using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateActiveCustomerCommand : ICommand
{
    private CardDeck _customerDeck;
    private CustomerCardData _customerCard;
    private ActiveCustomerSet _customerSet;
    private int _slotIndex;

    private CustomerCardState _savedCustomerState;

    public static CreateActiveCustomerCommand Create(
        int slotIndex,
        CardDeck customerDeck,
        ActiveCustomerSet customerSet
        )
    {
        CreateActiveCustomerCommand command = new CreateActiveCustomerCommand();
        command._customerDeck = customerDeck;
        command._slotIndex = slotIndex;
        command._customerSet = customerSet;
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
        _savedCustomerState = _customerSet.GetCustomerAtSlot(_slotIndex);

        CustomerCardData card = _customerDeck.Pop() as CustomerCardData;
        CustomerCardState newState = CustomerCardState.Create(card);
        _customerSet.SetCustomerAtSlot(_slotIndex, newState);
    }

    public void Undo()
    {
        CustomerCardState undoState = _customerSet.GetCustomerAtSlot(_slotIndex);
        _customerDeck.Push(undoState.cardData);
        _customerSet.SetCustomerAtSlot(_slotIndex, _savedCustomerState);
    }
}
