
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
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

    public Rigidbody rb;

    public void Start()
    {
        rb.centerOfMass = new Vector3(0, 0.25f, 0);
    }

    private void FixedUpdate()
    {
        // TODO: return if PV not mine, Photon!!

        GetInput();
        Motor();
        Steering();
        UpdateWheels();
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

        brakeForce = isBreaking ? 3000f : 0f;
        LF_Col.brakeTorque = brakeForce;
        RF_Col.brakeTorque = brakeForce;
        LB_Col.brakeTorque = brakeForce;
        RB_Col.brakeTorque = brakeForce;
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
}