using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

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

        return deck;
    }

    public BaseCardData Pop()
    {
        BaseCardData card = Top;
        _cardList.Remove(card);

        return card;
    }

    public BaseCardData Top
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

    public BaseCardData Bottom
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

    public void Shuffle()
    {
        _cardList.Sort((a, b) => Guid.NewGuid().CompareTo(Guid.NewGuid()));
    }
    
}
