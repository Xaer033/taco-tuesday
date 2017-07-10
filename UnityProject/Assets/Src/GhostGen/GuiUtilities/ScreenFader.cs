using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Assertions;

public class ScreenFader : MonoBehaviour
{
    public const float kDefaultFadeDuration = 0.25f;


    private Image _fadeBG;
    private CanvasGroup _canvasGroup;
    private Action _endCallback;
    private Tween _currentTween;

    // Use this for initialization
    void Awake()
    {
        _fadeBG = GetComponentInChildren<Image>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public Tween FadeIn(float duration = kDefaultFadeDuration, Action callback = null)
    {
        return _fade(1.0f, 0.0f, duration, callback);
    }

    public Tween FadeOut(float duration = kDefaultFadeDuration, Action callback = null)
    {
        return _fade(0.0f, 1.0f, duration, callback);
    }

    public Tween FadeTo(float endAlpha, float duration = kDefaultFadeDuration, Action callback = null)
    {
        return _fade(_canvasGroup.alpha, endAlpha, duration, callback);
    }

    public Tween Fade(float startAlpha, float endAlpha, float duration = kDefaultFadeDuration, Action callback = null)
    {
        return _fade(startAlpha, endAlpha, duration, callback);
    }

    public Color color
    {
        get { return _fadeBG.color; }
        set { _fadeBG.color = value; }
    }

    private Tween _fade(float startAlpha, float endAlpha, float duration, Action callback)
    {
        Assert.IsNotNull(_canvasGroup);

        _canvasGroup.alpha = startAlpha;
        _endCallback = callback;
        if(_currentTween != null)
        {
            _currentTween.Kill();
        }

        _currentTween = _canvasGroup.DOFade(endAlpha, duration);
        _currentTween.OnComplete(OnFadeComplete);
        return _currentTween;
    }


    private void OnFadeComplete()
    {
        if(_endCallback != null)
        {
            _endCallback();
            _endCallback = null;
        }

        _currentTween = null;
    }
}

