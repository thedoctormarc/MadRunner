using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreLogic : MonoBehaviour
{
    Text text;
    float time;

    void Start()
    {
        text = GetComponent<Text>();
        time = 0.0f;
    }

    void Update()
    {
        time += Time.deltaTime;
        float minutes = time / 60.0f;
        float seconds = time % 60.0f;
        float milliseconds = time * 100.0f % 100.0f;
        string textTime = string.Format("{0:00}:{1:00}:{2:00}", (int)minutes, (int)seconds, (int)milliseconds);
        text.text = textTime.ToString();
    }
}
