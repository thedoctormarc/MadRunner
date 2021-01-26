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
    public float time_current;
    float time_total;
    public float time_best;

    GameObject start_counter;
    GameObject start_counter_gradient;

    // Other scores
    public GameObject total_score;
    public GameObject best_score;

    Text total_score_text;
    public Text best_score_text;

    public List<float> lap_times;

    void Start()
    {
        text = GetComponent<Text>();
        time_current = 0.0f;

        start_counter = GameObject.Find("StartGameCounter");
        start_counter_gradient = GameObject.Find("GradientIllumination");

        total_score_text = total_score.GetComponent<Text>();
        best_score_text = best_score.GetComponent<Text>();

        time_total = 0.0f;
        time_best = 0.0f;
    }

    void Update()
    {
        time_to_start_timer += Time.deltaTime;

        if (time_to_start_timer >= time_to_start)
            started = true;

        if(started)
        {
            // Current (Lap) Time
            time_current += Time.deltaTime; // This will be reset when lapping
            float minutes_current = time_current / 60.0f;
            float seconds_current = time_current % 60.0f;
            float milliseconds_current = time_current * 100.0f % 100.0f;
            string string_time_current = string.Format("{0:00}:{1:00}:{2:00}", (int)minutes_current, (int)seconds_current, (int)milliseconds_current);
            text.text = string_time_current;
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

    public void CalculateTotalTime()
    {
        time_total += Time.deltaTime;
        float minutes_total = time_total / 60.0f;
        float seconds_total = time_total % 60.0f;
        float milliseconds_total = time_total * 100.0f % 100.0f;
        string string_time_total = string.Format("{0:00}:{1:00}:{2:00}", (int)minutes_total, (int)seconds_total, (int)milliseconds_total);
        total_score_text.text = "Total Time: " + string_time_total;
    }
}
