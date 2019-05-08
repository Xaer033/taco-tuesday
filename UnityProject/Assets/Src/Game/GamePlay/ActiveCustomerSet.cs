using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

[Serializable]
public class ActiveCustomerSet
{
    public const int kMaxActiveCustomers = 4;

    private CustomerCardState[] _activeCustomerList = new CustomerCardState[kMaxActiveCustomers];
    private ActiveCustomerSet() { }

    public static ActiveCustomerSet Create()
    {
        return new ActiveCustomerSet();
    }
    
    public bool isAllSlotsEmpty
    {
        get
        {
            for(int i = 0; i < kMaxActiveCustomers; ++i)
            {
                if(IsSlotActive(i))
                {
                    return false;
                }
            }
            return true;
        }
    }
    public bool IsSlotActive(int slotIndex)
    {
        _boundsCheck(slotIndex);
        return _activeCustomerList[slotIndex] != null;
    }

    public CustomerCardState GetCustomerByIndex(int slotIndex)
    {
        _boundsCheck(slotIndex);
        return _activeCustomerList[slotIndex];
    }

    public void SetCustomerAtIndex(int slotIndex, CustomerCardState cardState)
    {
        _boundsCheck(slotIndex);
        if(cardState != null)
        {
            cardState.slotIndex = slotIndex;
        }
        _activeCustomerList[slotIndex] = cardState;
    }

    
    private void _boundsCheck(int slotIndex)
    {
        Debug.Assert(slotIndex < kMaxActiveCustomers, string.Format("{0} out of range", slotIndex));
        Debug.Assert(slotIndex >= 0);
    }

}
