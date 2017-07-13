using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;


[CreateAssetMenu]
public class NetworkManager : ScriptableObject, IPostInit
{
    public string gameVersion;

	public void PostInit()
    {

    }

    public void CleanUp()
    {
        if(PhotonNetwork.connected)
        {
            Disconnect();
        }
    }

    public bool Connect()
    {
        if (PhotonNetwork.connected)
        {
            Debug.Log("Network: Already Connected!");
            return false;
        }

        return PhotonNetwork.ConnectUsingSettings(gameVersion);
    }

    public void Disconnect()
    {
    
        PhotonNetwork.Disconnect();
    }

    public bool isConnected
    {
        get { return PhotonNetwork.connected; }
    }
}
