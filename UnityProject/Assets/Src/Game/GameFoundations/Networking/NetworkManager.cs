using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;


public class NetworkManager : Photon.PunBehaviour, IPostInit
{
    public const string kGameVersion = "0.1.0";

    public event Action onCreatedRoom;
    public event Action onJoinedLobby;
    public event Action onReceivedRoomListUpdate;

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

        //PhotonNetwork.autoJoinLobby = true;
        return PhotonNetwork.ConnectUsingSettings(kGameVersion);
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public bool isConnected
    {
        get { return PhotonNetwork.connected; }
    }

    /// PUN Callbacks
    public override void OnCreatedRoom()
    {
        if(onCreatedRoom != null)
        {
            onCreatedRoom();
        }
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        Debug.Log(string.Format("Error:{0}, {1}", codeAndMsg[0], codeAndMsg[1]));
    }

    public override void OnReceivedRoomListUpdate()
    {
        Debug.Log("-On Received Room list update: " + PhotonNetwork.GetRoomList().Length);
        safeCall(onReceivedRoomListUpdate);
    }

    public override void OnConnectedToMaster()
    {
        //Debug.Log("-Joining Lobby-");
        //PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby: " + PhotonNetwork.lobby.Type.ToString());
        if(onJoinedLobby != null)
        {
            onJoinedLobby();
        }
    }

    private void safeCall(Action callback)
    {
        if(callback != null)
        {
            callback();
        }
    }
}
