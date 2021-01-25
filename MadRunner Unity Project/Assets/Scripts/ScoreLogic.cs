using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreLogic : MonoBehaviour
{
    public bool started = false;
    public float time_to_start = 5.0f;
    float time_to_start_timer = 0.0f;

    Text text;
    float time;
    float time_total;
    float time_best;

    GameObject start_counter;
    GameObject start_counter_gradient;

    // Other scores
    public GameObject total_score;
    public GameObject best_score;

    Text total_score_text;
    Text best_score_text;

    void Start()
    {
        text = GetComponent<Text>();
        time = 0.0f;

        start_counter = GameObject.Find("StartGameCounter");
        start_counter_gradient = GameObject.Find("GradientIllumination");

        total_score_text = total_score.GetComponent<Text>();
        best_score_text = best_score.GetComponent<Text>();
    }

    void Update()
    {
        time_to_start_timer += Time.deltaTime;

        if (time_to_start_timer >= time_to_start)
            started = true;

        if(started)
        {
            time_total += Time.deltaTime;
            float minutes = time_total / 60.0f;
            float seconds = time_total % 60.0f;
            float milliseconds = time_total * 100.0f % 100.0f;
            string textTime = string.Format("{0:00}:{1:00}:{2:00}", (int)minutes, (int)seconds, (int)milliseconds);
            total_score_text.text = "Total Time: " + textTime;
        }

        if(!started)
        {
            start_counter.SetActive(true);
            start_counter_gradient.SetActive(true);
        }
        else
        {
            start_counter.SetActive(false);
            start_counter_gradient.SetActive(false);
        }
    }
}
