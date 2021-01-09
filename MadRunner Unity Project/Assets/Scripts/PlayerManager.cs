using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using System.Linq;  

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    [SerializeField]
    Vector3 polePosition;

    [SerializeField]
    [Range(4.5f, 6f)]
    float carDistanceX = 5f;

    [SerializeField]
    [Range(6.5f, 8f)]
    float carDistanceZ = 7f;

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

        object[] myCustomInitData = { carColor.r, carColor.g, carColor.b, carColor.a };

        int index = 0;

        // We assume that photon player list is already disordered
        for(int i = 0; i < PhotonNetwork.PlayerList.Length; ++i)
        {
            if(PhotonNetwork.PlayerList[i].NickName == PhotonNetwork.NickName)
            {
                index = i;
                break;
            }
        }

        // z forward positive, x right positive
        Vector3 position = polePosition;
        position.z -= index * carDistanceZ;
        position.x += (index % 2 == 0) ? 0f : carDistanceX;

        PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Car"), position, Quaternion.identity, 0, myCustomInitData);

    }

}