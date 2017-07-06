using System;
using System.Collections.Generic;
using GhostGen;

public class GameOverPopupController : BaseController 
{
    private GameOverPopup _gameOverPopup;
    private Action _onConfirm;

	public void Start(List<PlayerState> playerList, Action onConfirm)
    {
        _onConfirm = onConfirm;

        viewFactory.CreateAsync<GameOverPopup>("GameOverPopup", (view) =>
        {
            _gameOverPopup = view as GameOverPopup;
            List<PlayerState> sortedList = _getSortedList(playerList);
            
            _gameOverPopup.SetPlayerStates(sortedList);
            _gameOverPopup._confirmButton.onClick.AddListener(OnConfirm);            
        });
    }
    
    private List<PlayerState> _getSortedList(List<PlayerState> playerList)
    {
        List<PlayerState> sortedList = new List<PlayerState>(playerList);
        sortedList.Sort((a, b) =>
        {
            return b.score.CompareTo(a.score);
        });
        return sortedList;
    }
    private void OnConfirm()
    {
        _gameOverPopup._confirmButton.onClick.RemoveListener(OnConfirm);
        
        viewFactory.screenFader.FadeOut(0.5f, () =>
         {
             viewFactory.RemoveView(_gameOverPopup);
             if (_onConfirm != null)
             {
                 _onConfirm();
             }
         });
    }
}
