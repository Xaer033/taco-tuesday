using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GhostGen;

[System.Serializable]
public class ActiveCustomersView : UIView
{
    public Transform[] _activeSlotList;


    private CustomerCardView[] _cardViewList = new CustomerCardView[ActiveCustomerSet.kMaxActiveCustomers];

    void Awake()
    {
        Debug.Assert(_activeSlotList.Length == ActiveCustomerSet.kMaxActiveCustomers);
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
            cardView = (CustomerCardView)GameManager.cardResourceBank.CreateCardView(
                cardState.cardData, 
                _activeSlotList[index]);
            cardView.cardState = cardState;
            _cardViewList[index] = cardView;
        }
        _moveCardToSlot(index, cardView);
    }

    public CustomerCardView GetCardByIndex(int index)
    {
        _boundsCheck(index);
        return _cardViewList[index];
    }

    private void _moveCardToSlot(int index, CustomerCardView cardView)
    {
        cardView.transform.position = _activeSlotList[index].position;
    }
    
    private void _boundsCheck(int index)
    {
        Debug.Assert(index >= 0, "Index is less than 0!");
        Debug.Assert(index < ActiveCustomerSet.kMaxActiveCustomers, "Index is greater than slot container size");
    }
}
