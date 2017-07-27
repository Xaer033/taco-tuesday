using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChangeTurnEvent : System.Object
{
    public int previousPlayerIndex;
    public int activePlayerIndex;

    public int prevPlayerScore;

    public MoveRequest[] moveRequestList;

    public static ChangeTurnEvent Create(int prevPlayerIndex, int activePlayerIndex, int prevPlayerScore, MoveRequest[] movesList)
    {
        ChangeTurnEvent turn = new ChangeTurnEvent();
        turn.previousPlayerIndex = prevPlayerIndex;
        turn.activePlayerIndex = activePlayerIndex;
        turn.prevPlayerScore = prevPlayerScore;
        turn.moveRequestList = movesList;
        return turn;
    }
}
