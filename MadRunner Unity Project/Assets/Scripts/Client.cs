using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using System.Globalization;

public class Client : MonoBehaviourPunCallbacks
{
    public static Client instance;

    [SerializeField]
    InputField roomName;

    [SerializeField]
    InputField playerInputName;

    [SerializeField]
    Text roomMenuNameText;

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

    [SerializeField]
    public FlexibleColorPicker cp;

    bool spectator;

    private void Awake()
    {
        instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject);
    }

    public void ToggleSpectator() => spectator = !spectator;

    public bool IsSpectator() => spectator;

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        if(s.buildIndex == 1) // game aka circuit
        {
            if(spectator == false)
            {
                GameObject go = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);

                // pass the client car color to the car manager
                go.GetComponent<PlayerManager>().Set(cp.color);
            }

            // Instantiate dynamic rigidbody props
            if(PhotonNetwork.IsMasterClient == true)
            {
                InstantiateAllNetworkedProps();
            }
           
        }
    }

    void Start()
    {
        print("Client connecting to Server...");
        PhotonNetwork.GameVersion = "1.0";
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

        UpdatePlayerList();

        // Master client is the only one that can start the game
        playButton.SetActive(PhotonNetwork.IsMasterClient);

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
            if(i.RemovedFromList)
            {
                continue;
            }
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


   public override void OnMasterClientSwitched(Player newMasterClient)
    {
        playButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void AcceptChangeName() 
    {
        if(string.IsNullOrEmpty(playerInputName.text) == false)
        {
            PhotonNetwork.NickName = playerInputName.text;

            UpdatePlayerList();
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) // used to change the player list in the first scene
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            UpdatePlayerList();
        }
    }

    void UpdatePlayerList()
    {
        foreach (Transform t in playerListItemTransf)
        {
            Destroy(t.gameObject);
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            Instantiate(playerListItemPrefab, playerListItemTransf).GetComponent<PlayerListItem>().Set(p);
        }
    }

    void InstantiateAllNetworkedProps()
    {
        string path = Application.dataPath + "/Networked Data.txt";

        string[] data = File.ReadAllLines(path);

        // setup stuff so the code doesnt unexpectedly mess up at will
        CultureInfo c = CultureInfo.InvariantCulture;

        for (int i = 0; i < data.Length; ++i)
        {

            string line = data[i];
            string lineFormatted = line.ToString(c);
            string[] words = lineFormatted.Split(' ');

            // name
            string name = words[0];

            // position
            string posLine = words[1];
            string[] numbers = posLine.Split(':');

            float x = float.Parse(numbers[0], c);
            float y = float.Parse(numbers[1], c);
            float z = float.Parse(numbers[2], c);

            Vector3 position = new Vector3(x,y,z);

            // rotation
            string rotLine = words[2];
            string[] numbers2 = rotLine.Split(':');

            float r1 = float.Parse(numbers2[0], c);
            float r2 = float.Parse(numbers2[1], c);
            float r3 = float.Parse(numbers2[2], c);
            float r4 = float.Parse(numbers2[3], c);

            Quaternion rotation = new Quaternion(r1, r2, r3, r4);

            // Instantiation
            PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", name), position, rotation);

        }
    }

}

