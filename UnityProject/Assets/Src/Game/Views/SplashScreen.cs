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


    public override void OnViewOutro(Action finishedCallback)
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

        Tween t = Singleton.instance.gui.screenFader.FadeOut(0.5f);//fader.DOFade(1.0f, 1.0f);

        outroSeq.Insert(0.0f, titleShakePos);
        outroSeq.Insert(0.0f, titleShakeRot);
        outroSeq.Insert(0.0f, subtitleShakePos);
        outroSeq.Insert(0.0f, subtitleShakeRot);
        outroSeq.Append(t);
        outroSeq.AppendCallback(()=>finishedCallback());
    }
    
    private void _onIntroFinished()
    { 
        base.OnIntroTransitionFinished();
        gameObject.AddComponent<TapToTrigger>(); // Make this whole screen tap'able
    }
    
    private void _startIntroTweens()
    {
        const float Y_OFFSET = 50.0f;
        const float TWEEN_DURATION = 0.5f;

        Sequence introSeq = DOTween.Sequence();

        Tween fadeIn = Singleton.instance.gui.screenFader.FadeIn(TWEEN_DURATION);//fader.DOFade(1.0f, 1.0f);
        //fader.DOFade(0.0f, 1.0f);

        Sequence titleSeq = DOTween.Sequence();
        Tween titleFade = title.DOFade(1.0f, TWEEN_DURATION);
        Vector2 titleOldPos = title.rectTransform.anchoredPosition;
        title.rectTransform.anchoredPosition += Vector2.down * Y_OFFSET;
        Tween titleMove = title.rectTransform.DOAnchorPos(titleOldPos, TWEEN_DURATION);
        titleMove.SetEase(Ease.OutCubic);
        titleSeq.Insert(0, titleFade);
        titleSeq.Insert(0, titleMove);

        Sequence subtitleSeq = DOTween.Sequence();
        Tween subtitleFade = subtitle.DOFade(1.0f, TWEEN_DURATION);
        Vector2 subtitleOldPos = subtitle.rectTransform.anchoredPosition;
        subtitle.rectTransform.anchoredPosition += Vector2.down * Y_OFFSET;
        Tween subtitleMove = subtitle.rectTransform.DOAnchorPos(subtitleOldPos, TWEEN_DURATION);
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
