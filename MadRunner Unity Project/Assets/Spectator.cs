using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectator : MonoBehaviour
{
    [SerializeField]
    GameObject UI;
    GameObject lastEnabled;

    // Start is called before the first frame update
    void Start()
    {
        if(Client.instance.IsSpectator())
        {
            UI.SetActive(false);
            lastEnabled = transform.GetChild(0).gameObject;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            int realIndex = i + 1;
            if (Input.GetKeyDown("" + realIndex))
            {
                lastEnabled.SetActive(false);
                transform.GetChild(i).gameObject.SetActive(true);
                lastEnabled = transform.GetChild(i).gameObject;
            }
        }
    }
}
