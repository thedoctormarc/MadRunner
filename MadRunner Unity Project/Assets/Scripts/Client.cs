using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviourPunCallbacks
{
    void Start()
    {
        print("Client connecting to Server...");
        PhotonNetwork.GameVersion = "0.0.1";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print("Client connected to Master Server!");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        print("Client connected to Lobby!");
        MenuManager.instance.OpenMenu("Join");
    }

    public override void OnLeftLobby()
    {
        print("Client disconnected from Lobby");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Client disconnected from Server. Cause: " + cause.ToString());
    }
}
