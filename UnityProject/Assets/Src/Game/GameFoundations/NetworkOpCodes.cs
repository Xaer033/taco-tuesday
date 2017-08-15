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
    public const byte PLAYBACK_CONFIRMED        = 6;
    public const byte TIMER_UPDATE              = 7;
    public const byte FORCE_TURN_END            = 8;
}
