using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class PlayerState
{
    public int      index   { get; private set; }
    public string   name    { get; private set; }

    public int      points  { get; set; }

    public static PlayerState Create(int playerIndex, string name)
    {
        PlayerState player = new PlayerState();
        player.index = playerIndex;
        player.name = name;
        player.points = 0;
        return player;
    }
}
