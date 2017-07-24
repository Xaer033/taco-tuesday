
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class PointModifier
{
    public const string NONE = "";
    public const string X2 = "x2";
}

public class CardType
{
    public const string NONE        = "";
    public const string MEAT        = "meat";
    public const string VEGGIE      = "veggie";
    public const string TOPPING     = "topping";
    public const string CUSTOMER    = "customer";
}

public class CardUtility
{
    
    //public static CardType TypeFromString(string cardType)
    //{
    //    switch(cardType.ToLower())
    //    {
    //        case "meat": return CardType.Meat;
    //        case "veggie": return CardType.Veggie;
    //        case "topping": return CardType.Topping;
    //        case "customer": return CardType.Customer;
    //    }

    //    Debug.LogError(string.Format("We don't handle card type: {0}!", cardType));
    //    return CardType.None;
    //}

    public static int ApplyModifier(string modifier, int originalScore)
    {
        if(string.IsNullOrEmpty(modifier))
        {
            return originalScore;
        }

        switch(modifier)
        {
            case PointModifier.X2: return originalScore * 2;
        }
        
        return originalScore;
    }
}

[System.Serializable]
public abstract class BaseCardData : System.Object
{
    public string      id;
    public string      titleKey;
    public string      iconName;
    public string      cardType;
}

[System.Serializable]
public class CustomerCardData : BaseCardData
{
    public int      meatRequirement;     
    public int      veggieRequirement;   
    public int      toppingRequirement;  
    public int      baseReward;
    public string   modifier;
}

[System.Serializable]
public class IngredientCardData : BaseCardData
{
    public int      foodValue;
}


public class CardDataFactory
{
    public static BaseCardData CreateFromJson(JToken cardToken)
    {
        string type = cardToken.Value<string>("cardType");
        string id = cardToken.Value<string>("id");
        string titleKey = cardToken.Value<string>("titleKey");
        string iconName = cardToken.Value<string>("iconName");

        if (type == CardType.CUSTOMER)
        {
            CustomerCardData cData = new CustomerCardData();
            cData.cardType = type;
            cData.id = id;
            cData.titleKey = titleKey;
            cData.iconName = iconName;
            cData.baseReward = cardToken.Value<int>("baseReward");
            cData.meatRequirement = cardToken.Value<int>("meatRequirement");
            cData.veggieRequirement = cardToken.Value<int>("veggieRequirement");
            cData.toppingRequirement = cardToken.Value<int>("toppingRequirement");
            string modifier = cardToken.Value<string>("modifier");
            cData.modifier = (modifier == null) ? "" : modifier;
            return cData;
        }
        else
        {
            IngredientCardData iData = new IngredientCardData();
            iData.cardType = type;
            iData.id = id;
            iData.titleKey = titleKey;
            iData.iconName = iconName;
            iData.foodValue = cardToken.Value<int>("foodValue");
            return iData;
        }
    }

    public static string ToJson(BaseCardData cData, bool prettyPrint)
    {
        JToken token = JToken.FromObject(cData);
        Formatting format = (prettyPrint) ? Formatting.Indented : Formatting.None;
        return token.ToString(format);
    }    
}