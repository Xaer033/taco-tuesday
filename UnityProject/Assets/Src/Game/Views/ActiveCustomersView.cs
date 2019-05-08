using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GhostGen;

[System.Serializable]
public class ActiveCustomersView : UIView
{
    public Transform[] _activeSlotList;


    //private CustomerCardState[] _cardStateList = new CustomerCardState[ActiveCustomerSet.kMaxActiveCustomers];
    private CustomerCardView[] _cardViewList = new CustomerCardView[ActiveCustomerSet.kMaxActiveCustomers];

    void Awake()
    {
        Debug.Assert(_activeSlotList.Length == ActiveCustomerSet.kMaxActiveCustomers);
        foreach(Transform t in _activeSlotList)
        {
            Debug.Assert(t != null);
        }
    }

    public void RemoveCardByIndex(int index)
    {
        _boundsCheck(index);
        _cardViewList[index].gameObject.SetActive(false);
        //Singleton.instance.viewFactory.RemoveView(_cardViewList[index]);
    }

    public void SetCardByIndex(int index, CustomerCardView cardView)
    {
        _boundsCheck(index);
        cardView.gameObject.SetActive(true);

        if (_cardViewList[index] != cardView)
        {
            _cardViewList[index] = cardView;
            invalidateFlag |= InvalidationFlag.STATIC_DATA;
        }
    }

    public CustomerCardView GetCardByIndex(int index)
    {
        _boundsCheck(index);
        return _cardViewList[index];
    }


    protected override void OnViewUpdate()
    {
        base.OnViewUpdate();
        if(IsInvalid(InvalidationFlag.ALL))
        {
            for (int i = 0; i < ActiveCustomerSet.kMaxActiveCustomers; ++i)
            {
                CustomerCardView cardView = _cardViewList[i];
                if(cardView != null)
                {
                    cardView.invalidateFlag = InvalidationFlag.ALL;
                }
            }
        }
    }

    private CustomerCardView _processCustomerCard(int index, CustomerCardView view)
    {
        _moveCardToSlot(index, view);
        return view;
    }

    private void _moveCardToSlot(int index, CustomerCardView cardView) // TODO: Tween or do something cool!
    {
        cardView.transform.position = _activeSlotList[index].position;
    }
    
    private void _boundsCheck(int index)
    {
        Debug.Assert(index >= 0, "Index is less than 0!");
        Debug.Assert(index < ActiveCustomerSet.kMaxActiveCustomers, "Index is greater than slot container size");
    }
}
