using System;
using UnityEngine;
using GhostGen;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class PassInterlude : UIView 
{
    private const float TWEEN_DURATION = 0.5f;

    public CanvasGroup _canvasGroup;
    public Button _confirmButton;
    public TextMeshProUGUI _instructionLbl;

    private string _instructionText;

    void Start()
    {
        _startIntroTween();
    }

    public string instructionText
    {
        set
        {
            _instructionText = value;
            invalidateFlag = InvalidationFlag.STATIC_DATA;
        }
    }

    protected override void OnViewUpdate()
    {
        base.OnViewUpdate();
        if(IsInvalid(InvalidationFlag.STATIC_DATA))
        {
            _instructionLbl.text = _instructionText;
        }
    }

    public override void OnViewOutro(Action finishCallback)
    {
        Sequence seq = DOTween.Sequence();

        Vector3 targetPos = transform.localPosition + Vector3.down * 100;
        Tween moveTween = transform.DOLocalMoveY(targetPos.y, TWEEN_DURATION);
        moveTween.SetEase(Ease.OutQuint);

        Tween fadeTween = _canvasGroup.DOFade(0.0f, TWEEN_DURATION);

        seq.Insert(0.0f, moveTween);
        seq.Insert(0.0f, fadeTween);
        seq.AppendCallback(() =>
        {
            if(finishCallback != null)
            {
                finishCallback();
            }
        });
    }

    private void _startIntroTween()
    {
        Sequence seq = DOTween.Sequence();

        Vector3 savedScale = transform.localScale;
        transform.localScale = savedScale * 0.8f;
        Tween scaleTween = transform.DOScale(savedScale, TWEEN_DURATION);
        Tween fadeTween = _canvasGroup.DOFade(1.0f, TWEEN_DURATION);
        seq.Insert(0.0f, scaleTween);
        seq.Insert(0.0f, fadeTween);
        seq.AppendCallback(() =>
        {
            OnIntroTransitionFinished();
        });
    }
}
