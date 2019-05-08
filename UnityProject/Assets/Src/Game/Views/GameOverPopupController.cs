using System;
using System.Collections.Generic;
using GhostGen;

public class GameOverPopupController : BaseController 
{
    private GameOverPopup _gameOverPopup;
    private Action _onConfirm;

	public void Start(List<PlayerMatchRank> playerList, Action onConfirm)
    {
        _onConfirm = onConfirm;

        viewFactory.CreateAsync<GameOverPopup>("GameOverPopup", (view) =>
        {
            _gameOverPopup = view as GameOverPopup;
            List<PlayerMatchRank> sortedList = _getSortedList(playerList);
            
            _gameOverPopup.SetPlayerStates(sortedList);
            _gameOverPopup._confirmButton.onClick.AddListener(OnConfirm);            
        });
    }
    
    private List<PlayerMatchRank> _getSortedList(List<PlayerMatchRank> playerList)
    {
        List<PlayerMatchRank> sortedList = new List<PlayerMatchRank>(playerList);
        sortedList.Sort((a, b) =>
        {
            if(a.score == b.score)
            {
                if(a.cardCount == b.cardCount)
                {
                    return b.positiveCardCount.CompareTo(a.positiveCardCount);
                }
                return b.cardCount.CompareTo(a.cardCount);
            }
            return b.score.CompareTo(a.score);        
        });
        return sortedList;
    }

    private void OnConfirm()
    {
        _gameOverPopup._confirmButton.onClick.RemoveListener(OnConfirm);
        
        Singleton.instance.gui.screenFader.FadeOut(0.5f, () =>
        {
            viewFactory.RemoveView(_gameOverPopup);
            if (_onConfirm != null)
            {
                _onConfirm();
            }
        });
    }
}
