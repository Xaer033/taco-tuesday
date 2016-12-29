using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class CardDeck : System.Object
{
    public bool LoadDeckFromJson(string jsonStr)
    {
        bool result = false;
        JArray jsonDeck = JArray.Parse(jsonStr);

        Debug.Log("Name: " + jsonDeck[0]["name"].ToString());
        return result;
    }
}
