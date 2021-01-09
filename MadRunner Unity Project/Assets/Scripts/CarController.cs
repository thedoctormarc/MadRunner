﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Realtime;
using Photon.Pun;

public class CarController : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    private float horizontalInput;
    private float verticalInput;
    private float steerAngle;
    private bool isBreaking;

    public WheelCollider LF_Col, RF_Col, LB_Col, RB_Col;
    public Transform LF, RF, LB, RB;

    public float maxSteeringAngle = 30f;
    public float motorForce = 50f;
    public float brakeForce = 0f;

    [Range(2.0f, 5.0f)]
    public float maxBrakeSpeed = 5.0f;

    public Rigidbody rb;

    [Range(0.25f, 2.0f)]
    public float centerOfMassHeight = 0.25f;

    private AudioSource aS;

    PhotonView PV;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;
        Material bodyMat = gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().materials[0];
        Color c = new Color((float)instantiationData[0], (float)instantiationData[1], (float)instantiationData[2], (float)instantiationData[3]);
        bodyMat.color = c;
    }

    public void Start()
    {
        PV = GetComponent<PhotonView>();

        if (!PV.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        }

        rb.centerOfMass = new Vector3(0, centerOfMassHeight, 0);
        aS = GetComponent<AudioSource>();
    }

    void OnValidate()
    {
        rb.centerOfMass = new Vector3(0, centerOfMassHeight, 0);
    }

    private void FixedUpdate()
    {
        // TODO: return if PV not mine, Photon!!
        if (!PV.IsMine)
        {
            return;
        }

        GetInput();
        Motor();
        Steering();
        UpdateWheels();
        AdjustAudio();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void Steering()
    {
        steerAngle = maxSteeringAngle * horizontalInput;
        LF_Col.steerAngle = steerAngle;
        RF_Col.steerAngle = steerAngle;
    }

    private void Motor()
    {
        LF_Col.motorTorque = verticalInput * motorForce;
        RF_Col.motorTorque = verticalInput * motorForce;

        float bForce = 0f;
        bForce = (isBreaking) ? brakeForce : 0f;

        LF_Col.brakeTorque = bForce;
        RF_Col.brakeTorque = bForce;
        LB_Col.brakeTorque = bForce;
        RB_Col.brakeTorque = bForce;
    }

    private void UpdateWheels()
    {
        UpdateWheelPos(LF_Col, LF);
        UpdateWheelPos(RF_Col, RF);
        UpdateWheelPos(LB_Col, LB);
        UpdateWheelPos(RB_Col, RB);
    }

    private void UpdateWheelPos(WheelCollider wheelCollider, Transform trans)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        trans.rotation = rot;
        trans.position = pos;
    }

    private void AdjustAudio() // TODO photon audio 
    {
        float maxCarSpeed = 47f;
        aS.volume  =  Math.Max(0.2f, (((rb.velocity.magnitude - 0f) * (1f - 0f)) / (maxCarSpeed - 0f)) + 0f); // volume depends on speed
        aS.pitch = 0.7f + aS.volume;

    }

}