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
    const int kMaxSmoothFrame = 5;
    const float kDragScale = 1.2f;

    private EventTrigger _eventTrigger;
    private Vector3 _inputOffset;
    private RectTransform _rectXform;
    private Tween _resetTween;

    private Vector3 _prevDragPosition;
    private Vector2[] _averageDelta = new Vector2[kMaxSmoothFrame];
    private int _deltaIndex = 0;
    private bool _isDragging = false;
    private Vector2 _dragDelta;

    private Vector3 _originalScale;
    private Tween _scaleTween;

    public PlayerHandView handView { get; set; }
    public Transform handSlot { get; set; }
    public Transform dragLayer { get; set; }

    void Awake()
    {
        _rectXform = (RectTransform)transform;
        _setup();

        for(int i = 0; i < kMaxSmoothFrame; ++i)
        {
            _averageDelta[i] = Vector2.zero;
        }
    }

    void Start()
    {
        OnIntroTransitionFinished();
    }
    
    public override void Update()
    {
        base.Update();

        if(_isDragging)
        {
            _updateDragRotation();
        }
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

    private void _setup()
    {
        _eventTrigger = GetComponent<EventTrigger>();
        _originalScale = transform.localScale;
    }

    public void onCardBeginDrag(BaseEventData e)
    {
        PointerEventData eventData = (PointerEventData)e;

        if(_resetTween != null) { _resetTween.Kill(true); }

        if (_scaleTween != null) { _scaleTween.Kill(); }
        _scaleTween = transform.DOScale(_originalScale * kDragScale, 0.82f);
        _scaleTween.SetEase(Ease.OutBack);
        _scaleTween.OnComplete(() => _scaleTween = null);

        transform.SetParent(dragLayer);

        Vector3 mPos = Input.mousePosition;
        mPos.z = GameManager.Get().guiCanvas.planeDistance;
        _inputOffset = _rectXform.position - Camera.main.ScreenToWorldPoint(mPos);
       
        handView.canvasGroup.blocksRaycasts = false;
        _isDragging = true;
    }

    public void onCardDrag(BaseEventData e)
    {
        PointerEventData eventData = (PointerEventData)e;
        Vector3 mPos = Input.mousePosition;
        mPos.z = GameManager.Get().guiCanvas.planeDistance;
        _rectXform.position = Camera.main.ScreenToWorldPoint(mPos) + _inputOffset;
        _rectXform.position = _rectXform.position + (Camera.main.transform.position - _rectXform.position) * 0.1f;
        const float kScaleFactor = 2.0f;

        _dragDelta = -eventData.delta * kScaleFactor;
    }

    private void _updateDragRotation()
    {
        int index = _deltaIndex++ % kMaxSmoothFrame;
        _averageDelta[index] = _dragDelta;

        Vector2 smoothDelta = Vector2.zero;
        for(int i = 0; i < kMaxSmoothFrame; ++i)
        {
            smoothDelta += _averageDelta[i];
        }

        smoothDelta /= kMaxSmoothFrame;
        smoothDelta = Vector2.Max(smoothDelta, -Vector2.one * 15.0f);
        smoothDelta = Vector2.Min(smoothDelta, Vector2.one * 15.0f);
        transform.localRotation = Quaternion.Slerp(Quaternion.Euler(-smoothDelta.y, smoothDelta.x, 0), transform.localRotation, Time.deltaTime);
        _dragDelta = Vector2.zero;
    }

    public void onCardEndDrag(BaseEventData e)
    {
        PointerEventData eventData = (PointerEventData)e;
        handView.canvasGroup.blocksRaycasts = true;
        _isDragging = false;

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
        
        if(_scaleTween != null) { _scaleTween.Kill(true); }
        _scaleTween = transform.DOScale(_originalScale, kTweenDuration * 0.756f);
        _scaleTween.OnComplete(() => _scaleTween = null);
    }
}
