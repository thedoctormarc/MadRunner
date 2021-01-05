using Photon.Pun;
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

}
