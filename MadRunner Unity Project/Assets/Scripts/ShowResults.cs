using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ShowResults : MonoBehaviour
{
    public GameObject results_item_prefab;
    public Transform results_item_transform;

    GameObject results_go;
    ResultManager results_component;

    void Start()
    {
        
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

    public void Set()
    {
        results_go = GameObject.Find("Results");
        results_component = results_go.GetComponent<ResultManager>();

        foreach (Transform t in results_item_transform)
        {
            Destroy(t.gameObject);
        }

        foreach (KeyValuePair<string, string> attachStat in results_component.results)
        {
            PlayerListItem list_item = results_item_prefab.GetComponent<PlayerListItem>();
            list_item.text.text = attachStat.Key.ToString() + " - " + attachStat.Value.ToString();
            Instantiate(results_item_prefab, results_item_transform);
        }
    }
}
