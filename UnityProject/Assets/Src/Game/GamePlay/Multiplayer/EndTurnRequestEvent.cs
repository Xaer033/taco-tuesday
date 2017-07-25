using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This goes from the client to the server, requesting the moves indicated
[System.Serializable]
public class EndTurnRequestEvent : System.Object
{
    public int playerId;
    public MoveRequest[] moveRequestList;
    
    public static EndTurnRequestEvent Create(int playerId, MoveRequest[] moveList)
    {
        var request = new EndTurnRequestEvent();
        request.playerId = playerId;
        request.moveRequestList =  moveList;
        return request;
    }
}
