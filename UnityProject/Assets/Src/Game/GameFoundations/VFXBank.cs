using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum VFXType
{
    NONE = -1,
    CARD_HOVER
}

[CreateAssetMenu(fileName = "VFXBank", menuName = "Resource Banks/Visual FX")]
public class VFXBank : ScriptableObject
{
    public GameObject cardHover;


    public GameObject Create(
        VFXType type, 
        Vector3 position, 
        Quaternion rotation)
    {
        GameObject prefab = _getFXPrefab(type);
        Assert.IsNotNull(prefab);

        return Instantiate<GameObject>(prefab, position, rotation); 
    }

    private GameObject _getFXPrefab(VFXType type)
    {
        switch(type)
        {
            case VFXType.CARD_HOVER: return cardHover;
        }

        return null;
    }
	
}
