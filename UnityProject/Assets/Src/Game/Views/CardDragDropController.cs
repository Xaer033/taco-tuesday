using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardDragDropController
{
    const int kMaxSmoothFrame = 7;
    const float kDragScale = 1.2f;

    private Transform _target;
    private Transform _slot;
    private Transform _dragLayer;

    private Vector3 _inputOffset;

    private Vector2[] _averageDelta = new Vector2[kMaxSmoothFrame];
    private int _deltaIndex = 0;
    private bool _isDragging = false;
    private Vector2 _dragDelta;

    private Vector3 _originalScale;

    private Tween _scaleTween;
    private Tween _resetTween;
    private float _deltaTime;


    public CardDragDropController(
        Transform targetCard, 
        Transform slot, 
        Transform dragLayer)
    {
        _target = targetCard;
        _slot = slot;
        _dragLayer = dragLayer;

        _originalScale = _target.localScale;

        for (int i = 0; i < kMaxSmoothFrame; ++i)
        {
            _averageDelta[i] = Vector2.zero;
        }
    }
	
	// Update is called once per frame
	public void Step (float deltaTime)
    {
        if (_isDragging)
        {
            _updateDragRotation(deltaTime);
        }
    }

    public void OnDragBegin(PointerEventData eventData)
    {
        if (_resetTween != null) { _resetTween.Kill(true); }

        if (_scaleTween != null) { _scaleTween.Kill(); }
        _scaleTween = _target.DOScale(_originalScale * kDragScale, 0.82f);
        _scaleTween.SetEase(Ease.OutBack);
        _scaleTween.OnComplete(() => _scaleTween = null);
        

        Vector3 mPos = Input.mousePosition;
        mPos.z = GameManager.Get().guiCanvas.planeDistance;
        _inputOffset = _target.position - Camera.main.ScreenToWorldPoint(mPos);

        _target.SetParent(_dragLayer);
        _isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        const float kScaleFactor = 2.0f;

        Vector3 mPos = Input.mousePosition;
        mPos.z = GameManager.Get().guiCanvas.planeDistance;
        mPos = Camera.main.ScreenToWorldPoint(mPos) + _inputOffset;
        _target.position =mPos + (Camera.main.transform.position - mPos) * 0.1f;
       
        _dragDelta = -eventData.delta * kScaleFactor;
    }

    public void OnDragEnd(PointerEventData eventData)
    {
        const float kTweenDuration = 0.523f;

        _isDragging = false;

        for (int i = 0; i < kMaxSmoothFrame; ++i)
        {
            _averageDelta[i] = Vector2.zero;
        }
        _resetTween = _target.DOMove(_slot.position, kTweenDuration);
        _resetTween.SetEase(Ease.OutCubic);
        _resetTween.OnComplete(() =>
        {
            _resetTween = null;
            _target.SetParent(_slot);
        });

        _target.DOLocalRotateQuaternion(Quaternion.identity, kTweenDuration * 0.6f);

        if (_scaleTween != null) { _scaleTween.Kill(true); }
        _scaleTween = _target.DOScale(_originalScale, kTweenDuration * 0.756f);
        _scaleTween.OnComplete(() => _scaleTween = null);
    }

    private void _updateDragRotation(float deltaTime)
    {
        int index = _deltaIndex++ % kMaxSmoothFrame;
        _averageDelta[index] = _dragDelta;

        Vector2 smoothDelta = Vector2.zero;
        for (int i = 0; i < kMaxSmoothFrame; ++i)
        {
            smoothDelta += _averageDelta[i];
        }

        smoothDelta /= kMaxSmoothFrame;
        smoothDelta = Vector2.Max(smoothDelta, -Vector2.one * 15.0f);
        smoothDelta = Vector2.Min(smoothDelta, Vector2.one * 15.0f);

        _target.localRotation = Quaternion.Slerp(
            Quaternion.Euler(-smoothDelta.y, smoothDelta.x, 0), 
            _target.localRotation, 
            deltaTime
        );

        _dragDelta = Vector2.zero;
    }
}

