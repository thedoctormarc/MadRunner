using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedLogic : MonoBehaviour
{
    GameObject go_speed;
    Text text;
    Rigidbody rb;
    
    float multiplier = 0.0f;
    float imperial_system_multiplier = 1.8f;
    float metric_system_multiplier = 4.0f;

    GameObject game_config;
    Metrics metrics;
    bool is_imperial = false;

    void Start()
    {
        go_speed = GameObject.Find("Speed");
        game_config = GameObject.Find("GameConfig");

        text = go_speed.GetComponent<Text>();
        rb = GetComponent<Rigidbody>();
        metrics = game_config.GetComponent<Metrics>();

        text.text = (int)(rb.velocity.magnitude * metric_system_multiplier) + " Km/h";
    }


    void Update()
    {
        if(metrics.is_imperial_system)
            text.text = (int)(rb.velocity.magnitude * imperial_system_multiplier) + " MPH";
        else
            text.text = (int)(rb.velocity.magnitude * metric_system_multiplier) + " Km/h";
    }
}
