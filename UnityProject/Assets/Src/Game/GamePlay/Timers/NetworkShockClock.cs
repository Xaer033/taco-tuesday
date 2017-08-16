using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkShockClock : ShockClock 
{
    public override double GetTime()
    {
        return PhotonNetwork.time * 1000.0;
    }
}
