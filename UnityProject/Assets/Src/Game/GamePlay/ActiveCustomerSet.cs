﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;


public class ActiveCustomerSet
{
    public const int kMaxActiveCustomers = 4;

    private CustomerCardState[] _activeCustomerList = new CustomerCardState[kMaxActiveCustomers];
    private ActiveCustomerSet() { }

    public static ActiveCustomerSet Create()
    {
        return new ActiveCustomerSet();
    }
    

    public bool IsSlotActive(int slotIndex)
    {
        _boundsCheck(slotIndex);
        return _activeCustomerList[slotIndex] != null;
    }

    public CustomerCardState GetCustomeByIndex(int slotIndex)
    {
        _boundsCheck(slotIndex);
        return _activeCustomerList[slotIndex];
    }

    public void SetCustomerAtIndex(int slotIndex, CustomerCardState cardState)
    {
        _boundsCheck(slotIndex);
        cardState.slotIndex = slotIndex;
        _activeCustomerList[slotIndex] = cardState;
    }

    
    private void _boundsCheck(int slotIndex)
    {
        Debug.Assert(slotIndex < kMaxActiveCustomers, string.Format("{0} out of range", slotIndex));
        Debug.Assert(slotIndex >= 0);
    }

}
