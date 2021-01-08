using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if(PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        Debug.Log("Player Controller Instantiated.");
        PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Car"), Vector3.zero, Quaternion.identity);
    }
}
