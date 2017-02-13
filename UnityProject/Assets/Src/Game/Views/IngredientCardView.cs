using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GhostGen;
using UnityEngine.EventSystems;

using DG.Tweening;

[System.Serializable]
[RequireComponent(typeof(EventTrigger))]
public sealed class IngredientCardView : 
    BaseCardView, 
    IBeginDragHandler, 
    IEndDragHandler, 
    IDragHandler
{
    private CardDragDropController _dragDropController;

    public EventTrigger eventTrigger;

    public PlayerHandView handView { get; set; }
    public Transform handSlot { get; set; }
    public Transform dragLayer { get; set; }
    public bool isDropSuccessfull { get; set; } 
   
    public bool isDragging
    {
        get { return _dragDropController.isDragging; }
    }

    void Awake()
    {
        eventTrigger = GetComponent<EventTrigger>();
        isDropSuccessfull = false;
    }

    void Start()
    {
        _setup();
        OnIntroTransitionFinished();
    }
    
    public override void Update()
    {
        base.Update();
        _dragDropController.Step(Time.deltaTime);
    }

    protected override void OnViewUpdate()
    {
        base.OnViewUpdate();

        if( _cardData != null && IsInvalid(InvalidationFlag.STATIC_DATA) )
        {      
            _setIngredientCard((IngredientCardData)_cardData);            
        }        
    }

    private void _setIngredientCard(IngredientCardData ingredientData)
    {
        CardResourceBank cardBank = Singleton.instance.cardResourceBank;
        _backgroundImg.sprite = cardBank.GetIngredientBG(ingredientData.cardType);
        _cardTypeIcon.sprite = cardBank.GetIngredientTypeIcon(ingredientData.cardType);
        _foodValueLbl.text = string.Format("{0}", ingredientData.foodValue);
        _cardIcon.sprite = cardBank.GetMainIcon(ingredientData.iconName);
    }

    private void _setup()
    {
        _dragDropController = new CardDragDropController(transform, handSlot, dragLayer);
    }

    public void OnBeginDrag(PointerEventData e)
    {
        _dragDropController.OnDragBegin(e);
    }

    public void OnDrag(PointerEventData e)
    {
        _dragDropController.OnDrag(e);
    }
    
    public void OnEndDrag(PointerEventData e)
    {
        handView.canvasGroup.blocksRaycasts = true;
        _dragDropController.OnDragEnd(e, isDropSuccessfull);
    }
}
