using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GhostGen;
using UnityEngine.EventSystems;

[System.Serializable]
[RequireComponent(typeof(EventTrigger))]
public sealed class IngredientCardView : BaseCardView
{
    private EventTrigger _eventTrigger;
    private Vector2 _inputOffset;
    private RectTransform _rectXform;

    public Transform handSlot { get; set; }

    void Awake()
    {
        _rectXform = (RectTransform)transform;
        _setupEvents();
    }

    void Start()
    {
        OnIntroTransitionFinished();
    }
    
    protected override void OnViewUpdate()
    {
        base.OnViewUpdate();

        if( _cardData != null && IsInvalid(INVALIDATE_STATIC_DATA) )
        {      
            _setIngredientCard((IngredientCardData)_cardData);            
        }        
    }

    private void _setIngredientCard(IngredientCardData ingredientData)
    {
        CardResourceBank cardBank = GameManager.cardResourceBank;
        _backgroundImg.sprite = cardBank.GetIngredientBG(ingredientData.cardType);
        _cardTypeIcon.sprite = cardBank.GetIngredientTypeIcon(ingredientData.cardType);
        _foodValueLbl.text = string.Format("{0}", ingredientData.foodValue);
        _cardIcon.sprite = cardBank.GetMainIcon(ingredientData.iconName);
    }

    private void _setupEvents()
    {
        _eventTrigger = GetComponent<EventTrigger>();
        
    }

    public void onCardBeginDrag(BaseEventData e)
    {
        PointerEventData eventData = (PointerEventData)e;
        Debug.LogFormat("Begin Drag: {0}", cardData.id);
        _inputOffset = _rectXform.anchoredPosition - eventData.pressPosition;
    }

    public void onCardDrag(BaseEventData e)
    {
        PointerEventData eventData = (PointerEventData)e;
        _rectXform.anchoredPosition = eventData.position + _inputOffset;
    }

    public void onCardEndDrag(BaseEventData e)
    {
        PointerEventData eventData = (PointerEventData)e;
        Debug.LogFormat("End Drag: {0}", cardData.id);

        transform.position = transform.parent.position;
    }
}
