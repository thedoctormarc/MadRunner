using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    public RoomInfo roomInfo;

    public void Set(RoomInfo roomInfo)
    {
        this.roomInfo = roomInfo;
        text.text = roomInfo.Name;
    }

    public void OnClick()
    {
        Client.instance.JoinRoom(roomInfo);
    }

}

