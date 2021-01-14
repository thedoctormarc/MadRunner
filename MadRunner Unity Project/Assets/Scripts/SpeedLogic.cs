using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedLogic : MonoBehaviour
{
    GameObject go_speed;
    Text text;
    Rigidbody rb;

    void Start()
    {
        go_speed = GameObject.Find("Speed");

        text = go_speed.GetComponent<Text>();
        rb = GetComponent<Rigidbody>();

        text.text = (int)rb.velocity.magnitude + " Km/h";
    }


    void Update()
    {
        text.text = (int)rb.velocity.magnitude + " Km/h";
    }
}
