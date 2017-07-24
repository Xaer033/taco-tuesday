using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkOpCodes
{
    public const byte READY_TOGGLE              = 1;
    public const byte INITIAL_GAME_STATE        = 2;
    public const byte PLAYER_TURN_COMPLETED     = 3;
    public const byte BEGIN_NEXT_PLAYER_TURN    = 4;
    public const byte MATCH_OVER                = 5;

}
