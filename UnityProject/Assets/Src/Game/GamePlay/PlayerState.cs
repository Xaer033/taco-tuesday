using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class PlayerState
{
    public const int kHandSize = 5;

    public int      index   { get; private set; }
    public string   name    { get; private set; }

    public int      points  { get; set; }

    public PlayerHand hand { get; private set; }
    public static PlayerState Create(int playerIndex, string name)
    {
        PlayerState player = new PlayerState();
        player.hand = PlayerHand.Create(kHandSize);
        player.index = playerIndex;
        player.name = name;
        player.points = 0;
        return player;
    }

 
}
