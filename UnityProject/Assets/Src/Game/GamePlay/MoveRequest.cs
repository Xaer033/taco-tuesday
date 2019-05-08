using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct MoveResult
{
    public MoveRequest request;
    public string usedIngredient;
    public int addedScore;
}


[System.Serializable]
public struct MoveRequest
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

    static public MoveRequest Invalid()
    {
        return Create(-1, -1, -1);
    }

    public override bool Equals(object obj)
    {
        return obj is MoveRequest && this == (MoveRequest)obj;
    }

    public override int GetHashCode()
    {
        return playerIndex.GetHashCode() ^ handSlot.GetHashCode() ^ customerSlot.GetHashCode();
    }

    public static bool operator ==(MoveRequest x, MoveRequest y)
    {
        return x.playerIndex == y.playerIndex &&
                x.handSlot == y.handSlot &&
                x.customerSlot == y.customerSlot;
    }

    public static bool operator !=(MoveRequest x, MoveRequest y)
    {
        return !(x == y);
    }
}
