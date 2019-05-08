using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GhostGen;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
[RequireComponent(typeof(EventTrigger))]
public sealed class CustomerCardView : BaseCardView
{
    public GameObject _meatReqObj;
    public GameObject _veggieReqObj;
    public GameObject _toppingReqObj;
    public EventTrigger eventTrigger;

    private TextMeshProUGUI _meatReqLbl;
    private TextMeshProUGUI _veggieReqLbl;
    private TextMeshProUGUI _toppingReqLbl;
    
    private CustomerCardState _cardState = null;


    private void Awake()
    {
        eventTrigger = GetComponent<EventTrigger>();

        if(_meatReqObj != null)
            _meatReqLbl     = _meatReqObj.GetComponentInChildren<TextMeshProUGUI>();

        if (_veggieReqObj != null)
            _veggieReqLbl   = _veggieReqObj.GetComponentInChildren<TextMeshProUGUI>();

        if (_toppingReqObj != null)
            _toppingReqLbl  = _toppingReqObj.GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {
        OnIntroTransitionFinished();
    }

    public CustomerCardState cardState
    {
        set
        {
            _cardState = value;
            invalidateFlag |= InvalidationFlag.ALL;

            cardData = _cardState.cardData;
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
        if(cardState.cardData == null) { return; }

        if(IsInvalid(InvalidationFlag.DYNAMIC_DATA) )
        {    
            _foodValueLbl.text = string.Format("{0}", cardState.totalScore);
            _foodValueLbl.color = (cardState.totalScore > 0) ? Color.white : Color.red;
        }

        _setCustomerCard(
            cardState.GetIngredientReqLeft(CardType.MEAT),
            cardState.GetIngredientReqLeft(CardType.VEGGIE),
            cardState.GetIngredientReqLeft(CardType.TOPPING));

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
            _meatReqObj.SetActive(true);
        }

        if(veggieReq == 0)
        {
            _veggieReqObj.SetActive(false);
        }
        else
        {
            _veggieReqLbl.text = string.Format("x{0}", veggieReq);
            _veggieReqObj.SetActive(true);
        }

        if(toppingReq == 0)
        {
            _toppingReqObj.SetActive(false);
        }
        else
        {
            _toppingReqLbl.text = string.Format("x{0}", toppingReq);
            _toppingReqObj.SetActive(true);
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
