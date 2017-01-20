using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class CustomerController
{
    const int kMaxSlots = 4;

    public bool IsSlotActive(int slotIndex)
    {
        if (!_boundsCheck(slotIndex))
            return false;

        return _activeCustomerList[slotIndex] != null;
    }

    public CustomerCardState GetCustomerAtSlot(int slotIndex)
    {
        if (_boundsCheck(slotIndex))
            return null;

        return _activeCustomerList[slotIndex];
    }

    public void SetCustomerAtSlot(int slotIndex, CustomerCardState cardState)
    {
        if (!_boundsCheck(slotIndex))
            return;

        _activeCustomerList[slotIndex] = cardState;
    }


// ------------------------ Private Impl --------------------------

    private CustomerCardState[] _activeCustomerList = new CustomerCardState[kMaxSlots];


    private bool _boundsCheck(int slotIndex)
    {
        if (slotIndex >= kMaxSlots || slotIndex < 0)
        {
            Debug.LogError(string.Format("Slot id: {0} is outside of bounds", slotIndex));
            return false;
        }
        return true;
    }

}
