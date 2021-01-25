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

    void Start()
    {
        PV = GetComponent<PhotonView>();
        GameObject go_lap = GameObject.Find("Lap");
        Text text = go_lap.GetComponent<Text>();
        text.text = "LAP: " + current_lap + "/" + max_laps;

        score_logic = GameObject.Find("Score");
        score_logic_component = score_logic.GetComponent<ScoreLogic>();
    }

    public void AddLap()
    {
        if (current_lap < max_laps)
        {
            ++current_lap;

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

        GameObject go_lap = GameObject.Find("Lap");
        Text text = go_lap.GetComponent<Text>();
        text.text = "LAP: " + current_lap + "/" + max_laps;
    }
}
