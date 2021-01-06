﻿using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Client : MonoBehaviourPunCallbacks
{
    public static Client instance;

    [SerializeField]
    TMP_InputField roomName;

    [SerializeField]
    TMP_Text roomMenuNameText;

    [SerializeField]
    Transform roomListItemTransf;

    [SerializeField]
    GameObject roomListItemPrefab;

    [SerializeField]
    Transform playerListItemTransf;

    [SerializeField]
    GameObject playerListItemPrefab;

    [SerializeField]
    GameObject playButton;

    private void Awake()
    {
        instance = this;
    }

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
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        print("Client connected to Lobby!");
        MenuManager.instance.OpenMenu("Join");
        PhotonNetwork.NickName = "Anonymous Pleb " + Random.Range(0,5000).ToString("0000");
    }

    public override void OnLeftLobby()
    {
        print("Client disconnected from Lobby");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Client disconnected from Server. Cause: " + cause.ToString());
    }

    public void CreateRoom()
    {
        if ((string.IsNullOrEmpty(roomName.text)) == false)
        {
            PhotonNetwork.CreateRoom(roomName.text);
            MenuManager.instance.OpenMenu("Loading");
        }
    }

    public override void OnJoinedRoom()
    {
        print("Client joined a Room!");
        MenuManager.instance.OpenMenu("Room");
        roomMenuNameText.text = PhotonNetwork.CurrentRoom.Name;

        int c = 0;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            print("player found. Total: " + (++c).ToString());
            Instantiate(playerListItemPrefab, playerListItemTransf).GetComponent<PlayerListItem>().Set(p);
        }

        // Master client is the only one that can start the game
        if (PhotonNetwork.IsMasterClient == false)
        {
            playButton.SetActive(false);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print("Room creation failed. Message: " + message);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.instance.OpenMenu("Loading");
    }

    public override void OnLeftRoom()
    {
        print("Client left the Room");
        MenuManager.instance.OpenMenu("Join");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.instance.OpenMenu("Loading");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform t in roomListItemTransf)
        {
            Destroy(t.gameObject);
        }

        foreach(RoomInfo i in roomList)
        {
            Instantiate(roomListItemPrefab, roomListItemTransf).GetComponent<RoomListItem>().Set(i);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListItemTransf).GetComponent<PlayerListItem>().Set(newPlayer);
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1); // load the game scene
    }

}
