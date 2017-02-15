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
    public Text playerScoreLbl;

    private int _playerScore = 0;

    void Awake()
    {
        canvasGroup.alpha = 0.0f;
    }

    void Start()
    {
        Tween introTween = canvasGroup.DOFade(1.0f, 1.0f).OnComplete(OnIntroTransitionFinished);
        introTween.SetDelay(0.5f);
    }

    public int playerScore
    {
        set
        {
            if(_playerScore != value)
            {
                _playerScore = value;
                invalidateFlag = InvalidationFlag.DYNAMIC_DATA;
            }
        }
    }

    protected override void OnViewUpdate()
    {
        base.OnViewUpdate();
        if(IsInvalid(InvalidationFlag.DYNAMIC_DATA))
        {
            playerScoreLbl.text = string.Format("Score: {0}", _playerScore); // TODO: Localize this!
        }
    }
    //------------------- Private Implementation -------------------
    //--------------------------------------------------------------

}

