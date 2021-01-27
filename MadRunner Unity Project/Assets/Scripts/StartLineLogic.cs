using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StartLineLogic : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        GameObject go = other.gameObject;

        if(go.GetComponent<CarController>().GetPhotonView().Owner == PhotonNetwork.LocalPlayer)
        {
            LapLogic ll = go.GetComponent<LapLogic>();
            ll.AddLap();
        }

    }
}
