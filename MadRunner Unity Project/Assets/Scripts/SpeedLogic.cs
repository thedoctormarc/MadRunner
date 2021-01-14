using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedLogic : MonoBehaviour
{
    GameObject go_speed;
    Text text;

    void Start()
    {
        go_speed = GameObject.Find("Speed");
        text = go_speed.GetComponent<Text>();
        text.text = transform.position.x + "Km/h";
    }


    void Update()
    {
        text.text = transform.position.x + "Km/h";
    }
}
