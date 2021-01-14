using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metrics : MonoBehaviour
{
    public bool is_imperial_system = false; // Imperial = MPH speed, otherwise is KM/H

    public void SwapImperialSystem() { is_imperial_system = !is_imperial_system; }
}
