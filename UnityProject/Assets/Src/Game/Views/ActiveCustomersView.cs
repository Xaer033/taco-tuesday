﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GhostGen;

[System.Serializable]
public class ActiveCustomersView : UIView
{
    public Transform[] _activeSlotList;


    private CustomerCardView[] _cardViewList = new CustomerCardView[CustomerController.kMaxActiveCustomers];

    void Awake()
    {
        Debug.Assert(_activeSlotList.Length == CustomerController.kMaxActiveCustomers);
        foreach(Transform t in _activeSlotList)
        {
            Debug.Assert(t != null);
        }
    }

    public void SetCardByIndex(int index, CustomerCardState cardState)
    {
        _boundsCheck(index);

        CustomerCardView cardView = _cardViewList[index];
        if (cardView != null)
        {
            _cardViewList[index].cardState = cardState;
        }
        else
        {
            cardView = (CustomerCardView)GameManager.cardResourceBank.CreateCardView(cardState.cardData, _activeSlotList[index]);
        }
        _moveCardToSlot(index, cardView);
    }


    private void _moveCardToSlot(int index, CustomerCardView cardView)
    {
        cardView.transform.position = _activeSlotList[index].position;
    }
    
    private void _boundsCheck(int index)
    {
        Debug.Assert(index >= 0, "Index is less than 0!");
        Debug.Assert(index < CustomerController.kMaxActiveCustomers, "Index is greater than slot container size");
    }
}
