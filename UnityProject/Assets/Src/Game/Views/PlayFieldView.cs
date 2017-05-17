using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using GhostGen;
using DG.Tweening;
using TMPro;

public class PlayFieldView : UIView
{
    [System.Serializable]
    public class PlayerInfo
    {
        public TextMeshProUGUI nameLbl;
        public TextMeshProUGUI scoreLbl;
    }

    public CanvasGroup canvasGroup;

    public Transform staticCardLayer;
    public Button confirmButton;
    public Button undoButton;

    public PlayerInfo[] playerInfoList;


    private int[]       _playerScores    = new int[GameLogic.kMaxPlayers];
    private string[]    _playerNames     = new string[GameLogic.kMaxPlayers];
    private int _activeIndex = -1;

    void Awake()
    {
        canvasGroup.alpha = 0.0f;
    }

    void Start()
    {
        Tween introTween = canvasGroup.DOFade(1.0f, 1.0f).OnComplete(OnIntroTransitionFinished);
        introTween.SetDelay(0.5f);
    }

    public void setPlayerScore(int playerIndex, int value)
    {
        Assert.IsTrue(playerIndex >= 0);
        Assert.IsTrue(playerIndex < _playerScores.Length);

        if(_playerScores[playerIndex] != value)
        {
            _playerScores[playerIndex] = value;
            invalidateFlag = InvalidationFlag.DYNAMIC_DATA;
        }
    }

    public void setPlayerName(int playerIndex, string value)
    {
        Assert.IsTrue(playerIndex >= 0);
        Assert.IsTrue(playerIndex < _playerScores.Length);

        if(_playerNames[playerIndex] != value)
        {
            _playerNames[playerIndex] = value;
            invalidateFlag = InvalidationFlag.DYNAMIC_DATA;
        }
    }

    public void setActivePlayer(int playerIndex)
    {
        Assert.IsTrue(playerIndex >= 0);
        Assert.IsTrue(playerIndex < playerInfoList.Length);

        if (_activeIndex != playerIndex)
        {
            if (_activeIndex >= 0)
            { 
                playerInfoList[_activeIndex].nameLbl.color = Color.white;
                playerInfoList[_activeIndex].scoreLbl.color = Color.white;
            }

            _activeIndex = playerIndex;

            playerInfoList[_activeIndex].nameLbl.color = Color.green;
            playerInfoList[_activeIndex].scoreLbl.color = Color.green;
        }
    }

    protected override void OnViewUpdate()
    {
        base.OnViewUpdate();
        if(IsInvalid(InvalidationFlag.DYNAMIC_DATA))
        {
            for(int i = 0; i < GameLogic.kMaxPlayers; ++i)
            {
                playerInfoList[i].scoreLbl.text = string.Format("Score: {0}", _playerScores[i]); // TODO: Localize this!
                playerInfoList[i].nameLbl.text = string.Format("Player: {0}", _playerNames[i]); // and this
            }
        }
    }
    //------------------- Private Implementation -------------------
    //--------------------------------------------------------------

}

