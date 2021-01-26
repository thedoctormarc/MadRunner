using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowResults : MonoBehaviour
{
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
}
