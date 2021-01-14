using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    bool is_paused = false;

    Image dark;

    void Start()
    {
        dark = GetComponent<Image>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            is_paused = !is_paused;

            if (is_paused)
            {
                Time.timeScale = 0.0f;
                dark.enabled = true;

                for (int i = 0; i < transform.childCount; ++i)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
            }
            else
            {
                Time.timeScale = 1.0f;
                dark.enabled = false;

                for (int i = 0; i < transform.childCount; ++i)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }
}
