using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum VFXType
{
    NONE = -1,
    CARD_HOVER,
    CARD_SLAM
}

[CreateAssetMenu(fileName = "VFXBank", menuName = "Resource Banks/Visual FX")]
public class VFXBank : ScriptableObject
{
    public GameObject cardHover;
    public GameObject cardSlam;

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
            case VFXType.CARD_HOVER:    return cardHover;
            case VFXType.CARD_SLAM:     return cardSlam;
        }

        return null;
    }
	
}
