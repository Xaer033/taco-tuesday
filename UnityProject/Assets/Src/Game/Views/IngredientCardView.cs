using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GhostGen;
using UnityEngine.EventSystems;

using DG.Tweening;

[System.Serializable]
[RequireComponent(typeof(EventTrigger))]
public sealed class IngredientCardView : BaseCardView
{
    private EventTrigger _eventTrigger;
    private Vector3 _inputOffset;
    private RectTransform _rectXform;
    private Tween _resetTween;

    public PlayerHandView handView { get; set; }
    public Transform handSlot { get; set; }
    public Transform dragLayer { get; set; }

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

        transform.SetParent(dragLayer);

        Vector3 mPos = Input.mousePosition;
        mPos.z = GameManager.Get().guiCanvas.planeDistance;

        _inputOffset = _rectXform.position - Camera.main.ScreenToWorldPoint(mPos);
        if (_resetTween != null)
        {
            _resetTween.Kill(true);
        }

        handView.canvasGroup.blocksRaycasts = false;
    }

    public void onCardDrag(BaseEventData e)
    {
        PointerEventData eventData = (PointerEventData)e;
        Vector3 mPos = Input.mousePosition;
        mPos.z = GameManager.Get().guiCanvas.planeDistance;
        _rectXform.position = Camera.main.ScreenToWorldPoint(mPos) + _inputOffset;
    }

    public void onCardEndDrag(BaseEventData e)
    {
        PointerEventData eventData = (PointerEventData)e;
        handView.canvasGroup.blocksRaycasts = true;

        _resetTween = transform.DOMove(handSlot.position, 0.532f);
        _resetTween.SetEase(Ease.OutCubic);
        _resetTween.OnComplete(() =>
        {
            _resetTween = null;
            transform.SetParent(handSlot);
        });
    }
}
