using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Realtime;
using Photon.Pun;

public class NetworkedObject : MonoBehaviourPunCallbacks
{

    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<PhotonView>().IsMine == false)
        {
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<BoxCollider>());
        }
    }
     
}
