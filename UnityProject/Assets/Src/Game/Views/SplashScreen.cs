using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using GhostGen;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SplashScreen : UIView 
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI subtitle;
    
	void Awake()
    {
        title.alpha = 0;
        subtitle.alpha = 0;
    }

    void Start()
    {
        _startIntroTweens();
    }


    public override void OnViewOutro(bool immediately, OnViewRemoved removedCallback)
    {
        Sequence outroSeq = DOTween.Sequence();

        Tween titleShakePos =  title.transform.DOShakePosition(0.4f, 15.0f, 22);
        titleShakePos.SetEase(Ease.OutCubic);
        Tween titleShakeRot = title.transform.DOShakeRotation(0.4f, 9.0f, 16);
        titleShakeRot.SetEase(Ease.OutCubic);

        Tween subtitleShakePos = subtitle.transform.DOShakePosition(0.4f, 16.0f, 22);
        subtitleShakePos.SetEase(Ease.OutCubic);
        Tween subtitleShakeRot = subtitle.transform.DOShakeRotation(0.4f, 8.0f, 16);
        subtitleShakeRot.SetEase(Ease.OutCubic);

        Tween t = Singleton.instance.viewFactory.screenFader.FadeOut(1.0f);//fader.DOFade(1.0f, 1.0f);

        outroSeq.Insert(0.0f, titleShakePos);
        outroSeq.Insert(0.0f, titleShakeRot);
        outroSeq.Insert(0.0f, subtitleShakePos);
        outroSeq.Insert(0.0f, subtitleShakeRot);
        outroSeq.Append(t);
        outroSeq.AppendCallback(()=>
        { 
            base.OnViewOutro(immediately, removedCallback);
        });
    }
    
    private void _onIntroFinished()
    { 
        base.OnIntroTransitionFinished();
        gameObject.AddComponent<TapToTrigger>(); // Make this whole screen tap'able
    }
    
    private void _startIntroTweens()
    {
        const float Y_OFFSET = 50.0f;

        Sequence introSeq = DOTween.Sequence();

        Tween fadeIn = Singleton.instance.viewFactory.screenFader.FadeIn(1.0f);//fader.DOFade(1.0f, 1.0f);
        //fader.DOFade(0.0f, 1.0f);

        Sequence titleSeq = DOTween.Sequence();
        Tween titleFade = title.DOFade(1.0f, 1.0f);
        Vector2 titleOldPos = title.rectTransform.anchoredPosition;
        title.rectTransform.anchoredPosition += Vector2.down * Y_OFFSET;
        Tween titleMove = title.rectTransform.DOAnchorPos(titleOldPos, 1.0f);
        titleMove.SetEase(Ease.OutCubic);
        titleSeq.Insert(0, titleFade);
        titleSeq.Insert(0, titleMove);

        Sequence subtitleSeq = DOTween.Sequence();
        Tween subtitleFade = subtitle.DOFade(1.0f, 1.0f);
        Vector2 subtitleOldPos = subtitle.rectTransform.anchoredPosition;
        subtitle.rectTransform.anchoredPosition += Vector2.down * Y_OFFSET;
        Tween subtitleMove = subtitle.rectTransform.DOAnchorPos(subtitleOldPos, 1.0f);
        subtitleMove.SetEase(Ease.OutCubic);
        subtitleSeq.Insert(0, subtitleFade);
        subtitleSeq.Insert(0, subtitleMove);

        introSeq.Insert(0, fadeIn);
        introSeq.Insert(0.5f, titleSeq);
        introSeq.Insert(0.9f, subtitleSeq);
        introSeq.AppendCallback(_onIntroFinished);
        introSeq.SetDelay(0.5f);
    }
}
