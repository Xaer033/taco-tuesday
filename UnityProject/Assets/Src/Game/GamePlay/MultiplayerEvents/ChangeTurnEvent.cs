using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChangeTurnEvent : System.Object
{
    public int previousPlayerIndex;
    public int activePlayerIndex;

    public int prevPlayerScore;

    public List<MoveRequest>    moveRequestList;
    public List<MoveResult>     moveResultList;

    public static ChangeTurnEvent Create(
        int prevPlayerIndex, 
        int activePlayerIndex,
        int prevPlayerScore,
        List<MoveRequest> requestList,
        List<MoveResult> resultList)
    {
        ChangeTurnEvent turn = new ChangeTurnEvent();
        turn.previousPlayerIndex = prevPlayerIndex;
        turn.activePlayerIndex = activePlayerIndex;
        turn.prevPlayerScore = prevPlayerScore;
        turn.moveRequestList = requestList;
        turn.moveResultList = resultList;
        return turn;
    }
}
