using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StartLineLogic : MonoBehaviour
{
    PhotonView PV;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (PV.IsMine)
        {
            GameObject go = other.gameObject;
            LapLogic ll = go.GetComponent<LapLogic>();
            ll.AddLap();
        }
    }
}
