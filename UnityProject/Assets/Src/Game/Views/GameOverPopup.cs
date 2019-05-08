using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GhostGen;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using UnityEngine.Assertions;

public class GameOverPopup : UIView
{
    public TextMeshProUGUI _title;
    public Button _confirmButton;
    public PlacingUI[] _placingUI;

    private List<PlayerMatchRank> _playerStateList;

    void Awake()
    {
        Assert.IsTrue(_placingUI.Length == PlayerGroup.kMaxPlayerCount);
    }

    void Start()
    {
        OnIntroTransitionFinished();
    }

    // Assumes the player states have been sorted from first to last
    public void SetPlayerStates(List<PlayerMatchRank> playerStateList)
    {
        Assert.IsTrue(playerStateList.Count <= PlayerGroup.kMaxPlayerCount);
        if(playerStateList != _playerStateList)
        {
            _playerStateList = playerStateList;
            invalidateFlag = InvalidationFlag.DYNAMIC_DATA;
        }
    }

    protected override void OnViewUpdate()
    {
        base.OnViewUpdate();
       
        if(IsInvalid(InvalidationFlag.DYNAMIC_DATA) && _playerStateList != null)
        {
            for (int i = 0; i < PlayerGroup.kMaxPlayerCount; ++i)
            {
                if (i < _playerStateList.Count)
                {
                    PlayerMatchRank state = _playerStateList[i];
                    _placingUI[i].playerName.text = state.name;
                    _placingUI[i].finalScore.text = string.Format("Score: {0}", state.score);
                }
                else
                {
                    _placingUI[i].gameObject.SetActive(true);
                }            
            }
        }
    }
}
