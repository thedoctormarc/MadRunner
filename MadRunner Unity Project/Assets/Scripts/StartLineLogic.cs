using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLineLogic : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        GameObject go = other.gameObject;
        LapLogic ll = go.GetComponent<LapLogic>();

        if (ll.current_lap < ll.max_laps)
        {
            ++ll.current_lap;
        }
    }
}
