using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LapLogic : MonoBehaviour
{
    PhotonView PV;

    public int max_laps = 3;
    public int current_lap = 1;

    GameObject score_logic; // To reset lap time
    ScoreLogic score_logic_component; // To reset lap time

    CarController cc;

    // For showing the final results of the race
    GameObject results;
    ShowResults results_component;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        GameObject go_lap = GameObject.Find("Lap");
        Text text = go_lap.GetComponent<Text>();
        text.text = "LAP: " + current_lap + "/" + max_laps;

        score_logic = GameObject.Find("Score");
        score_logic_component = score_logic.GetComponent<ScoreLogic>();

        cc = GetComponent<CarController>();

        results = GameObject.Find("Results");
        results_component = results.GetComponent<ShowResults>();
    }

    private void Update()
    {
        if(!cc.has_ended && score_logic_component.started)
            score_logic_component.CalculateTotalTime();

        CheckShowResults();
    }

    public void AddLap()
    {
        if (current_lap < max_laps)
        {
            ++current_lap;

            if(current_lap == 1)
            {
                score_logic_component.time_total = 0.0f;
            }

            if (current_lap != 1)
            {
                score_logic_component.lap_times.Add(score_logic_component.time_current);
                score_logic_component.lap_times.Sort();
                score_logic_component.time_best = score_logic_component.lap_times[0];

                float minutes_best = score_logic_component.time_best / 60.0f;
                float seconds_best = score_logic_component.time_best % 60.0f;
                float milliseconds_best = score_logic_component.time_best * 100.0f % 100.0f;
                string string_time_best = string.Format("{0:00}:{1:00}:{2:00}", (int)minutes_best, (int)seconds_best, (int)milliseconds_best);
                score_logic_component.best_score_text.text = "Best Lap: " + string_time_best;
            }

            score_logic_component.time_current = 0.0f;
        }
        else
        {
            if(current_lap == max_laps)
            {
                cc.has_ended = true;
                ++current_lap;
            }
        }

        GameObject go_lap = GameObject.Find("Lap");
        Text text = go_lap.GetComponent<Text>();

        if (!cc.has_ended)
        {
            text.text = "LAP: " + current_lap + "/" + max_laps;
        }
        else
        {
            text.text = "";
        }
    }

    void CheckShowResults()
    {
        if (current_lap > max_laps)
        {
            results_component.EnableChildren();
        }
    }
}
