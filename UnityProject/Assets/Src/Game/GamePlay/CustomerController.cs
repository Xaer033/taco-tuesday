using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;


public class CustomerController : BaseController
{
    public const int kMaxActiveCustomers = 4;


    private ActiveCustomersView _activeCustomers;

    public void Start(Action callback, Transform parent = null)
    {
        if(_activeCustomers != null)
        {
            viewFactory.RemoveView(_activeCustomers, true);
        }

        viewFactory.CreateAsync<ActiveCustomersView>(
            "ActiveCustomersView", 
            (view)=>
            {
                _activeCustomers = (ActiveCustomersView)view;
                callback();
            }, parent
        );
    }

    public bool IsSlotActive(int slotIndex)
    {
        _boundsCheck(slotIndex);
        return _activeCustomerList[slotIndex] != null;
    }

    public CustomerCardState GetCustomerAtSlot(int slotIndex)
    {
        _boundsCheck(slotIndex);
        return _activeCustomerList[slotIndex];
    }

    public void SetCustomerAtSlot(int slotIndex, CustomerCardState cardState)
    {
        _boundsCheck(slotIndex);
        _activeCustomerList[slotIndex] = cardState;

        _activeCustomers.SetCardByIndex(slotIndex, cardState);
    }


// ------------------------ Private Impl --------------------------

    private CustomerCardState[] _activeCustomerList = new CustomerCardState[kMaxActiveCustomers];


    private void _boundsCheck(int slotIndex)
    {
        Debug.Assert(slotIndex < kMaxActiveCustomers, string.Format("{0} out of range", slotIndex));
        Debug.Assert(slotIndex >= 0);
    }

}
