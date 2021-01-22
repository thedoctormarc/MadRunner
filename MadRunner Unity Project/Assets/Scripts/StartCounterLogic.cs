using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartCounterLogic : MonoBehaviour
{
    Text text;
    float timer = 5.0f;

    void Start()
    {
        text = GetComponent<Text>();
        gameObject.SetActive(false);
    }

    void Update()
    {
        timer -= Time.deltaTime;
        text.text = timer.ToString("#0");
    }
}
