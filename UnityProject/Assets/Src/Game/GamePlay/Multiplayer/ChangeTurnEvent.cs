using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChangeTurnEvent : System.Object
{
    public int previousPlayerIndex;
    public int activePlayerIndex;

    public List<MoveRequest> moveRequestList;
}
