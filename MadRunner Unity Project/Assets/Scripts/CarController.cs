using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Realtime;
using Photon.Pun;

public class CarController : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IPunObservable

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
    
    public struct NetworkMovementData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;

        public bool initialized;
    };

    public NetworkMovementData networkMovementData;

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
        aS = GetComponent<AudioSource>();

        if (!PV.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
    
        }
        else
        {
            rb.centerOfMass = new Vector3(0, centerOfMassHeight, 0);
        }

    }

    void OnValidate()
    {
        rb.centerOfMass = new Vector3(0, centerOfMassHeight, 0);
    }

    private void FixedUpdate()
    {
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

    void LateUpdate()
    {
        if (!PV.IsMine) // Lag compensation via transform: the local car moves towards the owner client's car data
        {
            if (InGameClient.instance.GetLagCompensation && networkMovementData.initialized)
            {
                float deltaPos = (transform.position - networkMovementData.position).magnitude;
                float deltaRot = (transform.rotation.eulerAngles - networkMovementData.rotation.eulerAngles).magnitude;

                if (deltaPos > InGameClient.instance.GetminDeltaPosThreshold)
                {
                    Debug.Log("Adjusting car position via lag compensation. Delta pos: " + deltaPos);
                    transform.position = Vector3.MoveTowards(transform.position, networkMovementData.position, Time.deltaTime * networkMovementData.velocity.magnitude);
                }


                if (deltaPos > InGameClient.instance.GetminDeltaRotThreshold)
                {
                    Debug.Log("Adjusting car rotation via lag compensation. Delta rot: " + deltaRot);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, networkMovementData.rotation, Time.deltaTime * 100);
                }

            }
            return;
        }
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

    private void AdjustAudio() // TODO photon audio --> use networked velocity instead of rb velocity if pv not mine
    {
        float maxCarSpeed = 47f;
        aS.volume  =  Math.Max(0.2f, (((rb.velocity.magnitude - 0f) * (1f - 0f)) / (maxCarSpeed - 0f)) + 0f); // volume depends on speed
        aS.pitch = 0.7f + aS.volume;

    }

    void OnCollisionEnter(Collision collision) // if colliding with a dynamic prop, transfer ownership from master client to our client, so they can interact  
    {
        PhotonView PV = collision.gameObject.GetComponent<PhotonView>();
        CarController CC = collision.gameObject.GetComponent<CarController>();

        if (PV && !CC)
        {
            Debug.Log("Car taking ownership of object from master so it can interact with it!");
            PV.TransferOwnership(PhotonNetwork.LocalPlayer);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (PV && InGameClient.instance.GetLagCompensation)
        {
            if (stream.IsWriting && PV.IsMine) // I am the owner, so sent data
            {
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(rb.velocity);
            }
            else if (!PV.IsMine) // get data from the owner
            {
                if(networkMovementData.initialized == false)
                {
                    networkMovementData.initialized = true; // first time receiving data
                }
                networkMovementData.position = (Vector3)stream.ReceiveNext();
                networkMovementData.rotation = (Quaternion)stream.ReceiveNext();
                networkMovementData.velocity = (Vector3)stream.ReceiveNext();

                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                networkMovementData.position += (networkMovementData.velocity * lag);
            }
        }
    }

}