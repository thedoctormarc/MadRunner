using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SpeedLogic : MonoBehaviour
{
    PhotonView PV;

    GameObject go_speed;
    Text text;
    Rigidbody rb;
    
    float multiplier = 0.0f;
    float imperial_system_multiplier = 2.02f;
    float metric_system_multiplier = 4.5f;

    GameObject game_config;
    Metrics metrics;
    bool is_imperial = false;

    void Start()
    {
        PV = GetComponent<PhotonView>();

        go_speed = GameObject.Find("Speed");
        game_config = GameObject.Find("GameConfig");

        text = go_speed.GetComponent<Text>();
        rb = GetComponent<Rigidbody>();
        metrics = game_config.GetComponent<Metrics>();

        if(PV.IsMine)
            text.text = (int)(rb.velocity.magnitude * metric_system_multiplier) + " Km/h";
    }


    void Update()
    {
        if (PV.IsMine)
        {
            if (metrics.is_imperial_system)
                text.text = (int)(rb.velocity.magnitude * imperial_system_multiplier) + " MPH";
            else
                text.text = (int)(rb.velocity.magnitude * metric_system_multiplier) + " Km/h";
        }
    }
}
