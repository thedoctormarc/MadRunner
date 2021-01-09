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
        
    }

    public void Set(Color carColor)
    {
        if (PV.IsMine)
        {
            CreateController(carColor);
        }
    }

    void CreateController(Color carColor)
    {
        Debug.Log("Player Controller Instantiated.");

        object[] myCustomInitData = { carColor.r, carColor.g, carColor.b, carColor.a} ;

        Vector3 position = new Vector3(Random.Range(-20.0f, 20.0f), 0, Random.Range(-20.0f, 20.0f));
        GameObject car = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Car"), position, Quaternion.identity, 0, myCustomInitData);

    }
}
