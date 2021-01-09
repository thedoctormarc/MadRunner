using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LapLogic : MonoBehaviour
{
    PhotonView PV;

    public int max_laps = 3;
    public int current_lap = 1;

    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (PV.IsMine)
        {
            GameObject go_lap = GameObject.Find("Lap");
            Text text = go_lap.GetComponent<Text>();
            text.text = current_lap.ToString() + "/" + max_laps.ToString();
        }
    }
}
