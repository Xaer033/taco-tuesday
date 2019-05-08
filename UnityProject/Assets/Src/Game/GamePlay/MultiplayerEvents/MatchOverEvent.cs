using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PlayerMatchRank
{
    public string   name;
    public int      score;  
    public int      cardCount;
    public int      positiveCardCount;

}

[System.Serializable]
public class MatchOverEvent : System.Object
{
    public List<PlayerMatchRank> playerRanking;

    public static MatchOverEvent Create(List<PlayerState> playerRanking)
    {
        MatchOverEvent turn = new MatchOverEvent();
        List<PlayerMatchRank> rankList = new List<PlayerMatchRank>(playerRanking.Count);

        for(int i = 0; i < playerRanking.Count; ++i)
        {
            PlayerState state = playerRanking[i];
            PlayerMatchRank playerRank = new PlayerMatchRank()
            {
                name = state.name,
                score = state.score,
                cardCount = state.deadCustomerStack.Count,
                positiveCardCount = state.positiveCustomerCount
            };
            
            rankList.Add(playerRank);
        }

        turn.playerRanking = rankList;
        return turn;
    }
}
