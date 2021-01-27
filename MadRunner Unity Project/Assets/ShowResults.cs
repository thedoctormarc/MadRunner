using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ShowResults : MonoBehaviour
{
    public GameObject results_item_prefab;
    public Transform results_item_transform;

    void Start()
    {
        foreach (Transform t in results_item_transform)
        {
            Destroy(t.gameObject);
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            Instantiate(results_item_prefab, results_item_transform).GetComponent<PlayerListItem>().Set(p);
        }
    }

    void Update()
    {

    }

    public void EnableChildren()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
