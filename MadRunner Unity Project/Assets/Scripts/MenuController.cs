using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    bool is_paused = false;

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(is_paused)
                Time.timeScale = 1.0f;
            else
                Time.timeScale = 0.0f;

            is_paused = !is_paused;
        }
    }
}
