using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StartLineLogic : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        GameObject go = other.gameObject;
        LapLogic ll = go.GetComponent<LapLogic>();
        ll.AddLap();

        CarController cc = go.GetComponent<CarController>();
    }
}
