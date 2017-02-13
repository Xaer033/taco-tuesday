using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GhostGen;
using UnityEngine.EventSystems;

[System.Serializable]
[RequireComponent(typeof(EventTrigger))]
public sealed class CustomerCardView : BaseCardView
{
    public GameObject _meatReqObj;
    public GameObject _veggieReqObj;
    public GameObject _toppingReqObj;
    public EventTrigger eventTrigger;

    private Text _meatReqLbl;
    private Text _veggieReqLbl;
    private Text _toppingReqLbl;
    
    private CustomerCardState _cardState = null;


    private void Awake()
    {
        eventTrigger = GetComponent<EventTrigger>();

        if(_meatReqObj != null)
            _meatReqLbl     = _meatReqObj.GetComponentInChildren<Text>();

        if (_veggieReqObj != null)
            _veggieReqLbl   = _veggieReqObj.GetComponentInChildren<Text>();

        if (_toppingReqObj != null)
            _toppingReqLbl  = _toppingReqObj.GetComponentInChildren<Text>();
    }

    void Start()
    {
        OnIntroTransitionFinished();
    }

    public CustomerCardState cardState
    {
        set
        {
            if(_cardState != value)
            {
                _cardState = value;
                invalidateFlag |= InvalidationFlag.DYNAMIC_DATA;
            }
        }
        get
        {
            return _cardState;
        }
       
    }

    protected override void OnViewUpdate()
    {
        base.OnViewUpdate();

        if(cardState == null) { return; }

        if(IsInvalid(InvalidationFlag.STATIC_DATA) )
        {    
            _foodValueLbl.text = string.Format("{0}", cardState.cardData.baseReward);
        }

        _setCustomerCard(
            cardState.GetIngredientReqLeft(CardType.Meat),
            cardState.GetIngredientReqLeft(CardType.Veggie),
            cardState.GetIngredientReqLeft(CardType.Topping));

    }

    private void _setCustomerCard(int meatReq, int veggieReq, int toppingReq)
    {
        if(meatReq == 0)
        {
            _meatReqObj.SetActive(false);
        }
        else
        {
            _meatReqLbl.text = string.Format("x{0}", meatReq);
        }

        if(veggieReq == 0)
        {
            _veggieReqObj.SetActive(false);
        }
        else
        {
            _veggieReqLbl.text = string.Format("x{0}", veggieReq);
        }

        if(toppingReq == 0)
        {
            _toppingReqObj.SetActive(false);
        }
        else
        {
            _toppingReqLbl.text = string.Format("x{0}", toppingReq);
        }
    }

    public void onCardDrop(BaseEventData e)
    {
        PointerEventData eventData = (PointerEventData)e;
        IngredientCardView ingredient = eventData.pointerDrag.GetComponent<IngredientCardView>();
        if (ingredient == null)
        {
            Debug.LogFormat("Drag Object: {0}", eventData.pointerDrag.name);
            return;
        }
        Debug.LogFormat("Dropped: {0} onto Customer: {1}", ingredient.cardData.id, cardData.id);
    }
}
