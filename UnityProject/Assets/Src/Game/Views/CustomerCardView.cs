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

    private Text _meatReqLbl;
    private Text _veggieReqLbl;
    private Text _toppingReqLbl;
    
    private CustomerCardState _cardState = null;

    private EventTrigger _eventTrigger;

    private void Awake()
    {

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
                invalidateFlag = INVALIDATE_DYNAMIC_DATA;
            }
        }
    }

    protected override void OnViewUpdate()
    {
        base.OnViewUpdate();

        if(_cardData == null)
        {
            return;
        }

        if(IsInvalid(INVALIDATE_STATIC_DATA) )
        {       
            _setCustomerCard((CustomerCardData)_cardData);          
        }

        if(IsInvalid(INVALIDATE_DYNAMIC_DATA) )
        {
            _meatReqLbl.text = string.Format("x{0}", _cardState.GetIngredientReqLeft(CardType.Meat));
            _veggieReqLbl.text = string.Format("x{0}", _cardState.GetIngredientReqLeft(CardType.Veggie));
            _toppingReqLbl.text = string.Format("x{0}", _cardState.GetIngredientReqLeft(CardType.Topping));
        }
    }

    private void _setCustomerCard(CustomerCardData customerData)
    {
        if(customerData.meatRequirement == 0)
        {
            _meatReqObj.SetActive(false);
        }
        else
        {
            _meatReqLbl.text = string.Format("x{0}", customerData.meatRequirement);
        }

        if(customerData.veggieRequirement == 0)
        {
            _veggieReqObj.SetActive(false);
        }
        else
        {
            _veggieReqLbl.text = string.Format("x{0}", customerData.veggieRequirement);
        }

        if(customerData.toppingRequirement == 0)
        {
            _toppingReqObj.SetActive(false);
        }
        else
        {
            _toppingReqLbl.text = string.Format("x{0}", customerData.toppingRequirement);
        }

        _foodValueLbl.text = string.Format("{0}", customerData.baseReward);
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
