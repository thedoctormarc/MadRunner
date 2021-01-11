using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameClient : MonoBehaviour
{
    public static InGameClient instance;

    [Header("Lag compensation Settings")]


    [Tooltip("Toggle in-game if you want")]
    [SerializeField]
    bool lagCompensation = true;

    [Tooltip("Minimum delta position between local player's car and owner's car when the local player starts to move towards the owner's object")]
    [SerializeField]
    [Range(0f, 0.1f)]
    float minDeltaPosThreshold = 0f;

    [Tooltip("Minimum delta rotation (angle magnitude) between local player's car and owner's car when the local player starts to move towards the owner's object")]
    [SerializeField]
    [Range(0f, 5f)]
    float minDeltaRotThreshold = 0f;


    public bool GetLagCompensation => lagCompensation;
    public float GetminDeltaPosThreshold => minDeltaPosThreshold;
    public float GetminDeltaRotThreshold => minDeltaRotThreshold;

    private void Awake()
    {
        instance = this;
    }
}
