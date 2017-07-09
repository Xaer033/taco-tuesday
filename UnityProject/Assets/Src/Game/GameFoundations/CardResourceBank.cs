﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using GhostGen;

public class CardResourceBank : ScriptableObject, IPostInit
{
    [System.Serializable]
    public class IngredientCardSprites
    {
        public Sprite background;
        public Sprite icon;
    }

    public IngredientCardSprites meatSprites;
    public IngredientCardSprites veggieSprites;
    public IngredientCardSprites toppingSprites;

    public BaseCardView customerCardView;
    public BaseCardView ingredientCardView;

    public Texture2D iconAtlas;


    private Dictionary<string, Sprite> _iconMap = new Dictionary<string, Sprite>();

    public void PostInit()
    { 
        Sprite[] sprites = Resources.LoadAll<Sprite>("Atlases/" + iconAtlas.name);
        foreach(Sprite s in sprites)
        {
            _iconMap.Add(s.name, s);
        }
    }

    public Sprite GetMainIcon(string iconName)
    {
        Sprite icon = null;
        if(!_iconMap.TryGetValue(iconName, out icon))
        {
            Debug.LogError(string.Format("Could not find icon named: {0}", iconName));
            return null;
        }
        return icon;
    }

    public Sprite GetIngredientTypeIcon(CardType type)
    {
        switch(type)
        {
            case CardType.Meat:      return meatSprites.icon;
            case CardType.Veggie:    return veggieSprites.icon;
            case CardType.Topping:   return toppingSprites.icon;
        }

        Debug.LogError(string.Format("Can't find Icon for type: {0}", type));
        return null;
    }

    public Sprite GetIngredientBG(CardType type)
    {
        switch (type)
        {
            case CardType.Meat:      return meatSprites.background;
            case CardType.Veggie:    return veggieSprites.background;
            case CardType.Topping:   return toppingSprites.background;
        }

        Debug.LogError(string.Format("Can't find background for type: {0}", type));
        return null;
    }

    public BaseCardView CreateCardView(BaseCardData cardData, Transform cParent)
    {
        BaseCardView prefab = (cardData.cardType == CardType.Customer) ?
                           customerCardView : ingredientCardView;

        BaseCardView view = Instantiate<BaseCardView>(prefab, cParent, false);
        view.cardData = cardData;

        return view;
    }
}
