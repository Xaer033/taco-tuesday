using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MoveRequest
{
    public int playerIndex;
    public int handSlot;
    public int customerSlot;

    public static MoveRequest Create(int index, int handSlot, int customerSlot)
    {
        MoveRequest request = new MoveRequest();
        request.playerIndex = index;
        request.handSlot = handSlot;
        request.customerSlot = customerSlot;
        return request;
    }
}

// This goes from the client to the server, requesting the moves indicated
[System.Serializable]
public class EndTurnRequestEvent : System.Object
{
    public MoveRequest[] moveRequestList = new MoveRequest[PlayerState.kMaxCardsPerTurn];
    
    public static EndTurnRequestEvent Create(MoveRequest req1, MoveRequest req2)
    {
        var request = new EndTurnRequestEvent();
        request.moveRequestList[0] = req1;
        request.moveRequestList[1] = req2;
        return request;
    }
}
