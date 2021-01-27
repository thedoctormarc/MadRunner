using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class ResultsJoined : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    Dictionary<string, float> res;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        res = new Dictionary<string, float>();
    }

    public void CallAddResult(string name, float total_time)
    {
        AddResult(name, total_time);
        PV.RPC("AddResult", RpcTarget.Others, name, total_time);
    }

    [PunRPC]
    void AddResult(string name, float total_time)
    {
        res.Add(name, total_time);
    }
}
