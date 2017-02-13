using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GhostGen;
using DG.Tweening;

public class PlayFieldView : UIView
{
    public CanvasGroup canvasGroup;

    public Transform staticCardLayer;
    public Button confirmButton;
    public Button undoButton;
      
    void Awake()
    {
        canvasGroup.alpha = 0.0f;
    }

    void Start()
    {
        Tween introTween = canvasGroup.DOFade(1.0f, 1.0f).OnComplete(OnIntroTransitionFinished);
        introTween.SetDelay(0.5f);
    }

//------------------- Private Implementation -------------------
//--------------------------------------------------------------

}

