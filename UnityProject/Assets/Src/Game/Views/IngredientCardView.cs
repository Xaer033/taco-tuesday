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
    const int kMaxSmoothFrame = 35;

    private EventTrigger _eventTrigger;
    private Vector3 _inputOffset;
    private RectTransform _rectXform;
    private Tween _resetTween;
    private Vector3 _prevDragPosition;
    private Vector2[] _averageDelta = new Vector2[kMaxSmoothFrame];
    private int _deltaIndex = 0;


    public PlayerHandView handView { get; set; }
    public Transform handSlot { get; set; }
    public Transform dragLayer { get; set; }

    void Awake()
    {
        _rectXform = (RectTransform)transform;
        _setupEvents();

        for(int i = 0; i < kMaxSmoothFrame; ++i)
        {
            _averageDelta[i] = Vector2.zero;
        }
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

        if (_resetTween != null)
        {
            _resetTween.Kill(true);
        }

        transform.SetParent(dragLayer);

        Vector3 mPos = Input.mousePosition;
        mPos.z = GameManager.Get().guiCanvas.planeDistance;

        _inputOffset = _rectXform.position - Camera.main.ScreenToWorldPoint(mPos);
       
        handView.canvasGroup.blocksRaycasts = false;
    }

    public void onCardDrag(BaseEventData e)
    {
        PointerEventData eventData = (PointerEventData)e;
        Vector3 mPos = Input.mousePosition;
        mPos.z = GameManager.Get().guiCanvas.planeDistance;
        _rectXform.position = Camera.main.ScreenToWorldPoint(mPos) + _inputOffset;

        Vector2 smoothDelta = _updateSmoothDelta(eventData.delta);
        smoothDelta = Vector2.Max(smoothDelta, -Vector2.one * 5.0f);
        smoothDelta = Vector2.Min(smoothDelta, Vector2.one * 5.0f);
        transform.localRotation = Quaternion.Slerp(Quaternion.Euler(-smoothDelta.y, smoothDelta.x, 0), transform.localRotation, Time.deltaTime);
    }

    private Vector2 _updateSmoothDelta(Vector2 delta)
    {
        const float kScaleFactor = 1.0f;
        
        int index = _deltaIndex % kMaxSmoothFrame;
        _averageDelta[index] = delta * kScaleFactor;

        Vector2 smoothDelta = Vector2.zero;
        for(int i = 0; i < kMaxSmoothFrame; ++i)
        {
            smoothDelta += _averageDelta[index];
        }
        smoothDelta /= kMaxSmoothFrame;
        
        _deltaIndex++;
        return smoothDelta;
    }
    public void onCardEndDrag(BaseEventData e)
    {
        PointerEventData eventData = (PointerEventData)e;
        handView.canvasGroup.blocksRaycasts = true;

        for (int i = 0; i < kMaxSmoothFrame; ++i)
        {
            _averageDelta[i] = Vector2.zero;
        }

        const float kTweenDuration = 0.523f;

        _resetTween = transform.DOMove(handSlot.position, kTweenDuration);
        _resetTween.SetEase(Ease.OutCubic);
        _resetTween.OnComplete(() =>
        {
            _resetTween = null;
            transform.SetParent(handSlot);
        });

        transform.DOLocalRotateQuaternion(Quaternion.identity, kTweenDuration * 0.6f);
    }
}
