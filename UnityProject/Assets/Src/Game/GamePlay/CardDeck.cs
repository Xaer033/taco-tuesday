﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

[System.Serializable]
public class CardDeck : System.Object
{
    private List<BaseCardData> _cardList = new List<BaseCardData>();

    public List<BaseCardData> cardList { get { return _cardList; } }


    public static CardDeck FromFile(string path)
    {
        TextAsset ingredientDeckJson = Resources.Load<TextAsset>(path);
        return CardDeck.FromJson(ingredientDeckJson.text);
    }

    public static CardDeck FromJson(string jsonStr)
    {
        CardDeck    deck        = new CardDeck();
        JArray      jsonDeck    = JArray.Parse(jsonStr);

        for (int i = 0; i < jsonDeck.Count; ++i)
        {
            JToken card = jsonDeck[i];
            int cardCount = card.Value<int>("cardCount");
            cardCount = (cardCount <= 0) ? 1 : cardCount;
            for (int j = 0; j < cardCount; ++j)
            { 
                deck._cardList.Add(CardDataFactory.CreateFromJson(card));
            }
        }

        //string scrambledDeck = CardDeck.ToJson(deck, true);
        //Debug.Log(scrambledDeck);
        return deck;
    }
    public static string ToJson(CardDeck deck, bool prettyPrint)
    {
        JArray token = JArray.FromObject(deck._cardList);
        Formatting format = (prettyPrint) ? Formatting.Indented : Formatting.None;
        return token.ToString(format);
    }


    public BaseCardData Pop()
    {
        BaseCardData card = top;
        _cardList.Remove(card);

        return card;
    }

    public void Push(BaseCardData card)
    {
        _cardList.Add(card);
    }

    public BaseCardData top
    {
        get
        {
            if (_cardList.Count == 0)
            {
                return null;
            }

            return _cardList[_cardList.Count - 1];
        }
    }

    public BaseCardData bottom
    {
        get
        {
            if (_cardList.Count == 0)
            {
                return null;
            }

            return _cardList[0];
        }
    }

    public void Shuffle(int randomSeed = -1)
    {
        System.Random random = new System.Random(randomSeed);
        
        _cardList.Sort((a, b) =>
        {
            return random.Next(0, 100).CompareTo(random.Next(0, 100));
        });
    }

    public bool isEmpty
    {
        get
        {
            return _cardList.Count == 0;
        }
    }
    
}
