using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ResultManager : MonoBehaviourPunCallbacks
{
    public Dictionary<string, string> results;
    PhotonView PV;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        results = new Dictionary<string, string>();
    }

    public void CallAddResult(string name, string bestLap)
    {
        AddResult(name, bestLap);
        PV.RPC("AddResult", RpcTarget.Others, name, bestLap);
    }

    [PunRPC]
    void AddResult(string name, string bestLap)
    {
        results.Add(name, bestLap);
    }

}
