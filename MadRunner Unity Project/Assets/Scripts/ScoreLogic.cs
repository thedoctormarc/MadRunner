using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreLogic : MonoBehaviour
{
    Text text;
    float total_time;

    void Start()
    {
        text = GetComponent<Text>();
        total_time = 0.0f;
    }

    void Update()
    {
        total_time += Time.deltaTime;
        text.text = total_time.ToString("#.00");
    }
}
