using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

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

    public GameObject rearviewImage, rearviewBorder;

    // Lights
    GameObject rear_left_light;
    GameObject rear_right_light;

    // UI
    GameObject playerNameText; 


    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;
        Material bodyMat = gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().materials[0];
        Color c = new Color((float)instantiationData[0], (float)instantiationData[1], (float)instantiationData[2], (float)instantiationData[3]);
        bodyMat.color = c;
    }

    public void Awake()
    {
        PV = GetComponent<PhotonView>();
        playerNameText = transform.Find("Canvas").Find("PlayerName").gameObject;
        playerNameText.GetComponent<Text>().text = PV.Owner.NickName;

        if (!PV.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject.transform.parent.gameObject); // destroy camera holder directly (both cameras)
            Destroy(rb);
        }
        else
        {
            GameManager.instance.onwPlayer = gameObject;
            playerNameText.transform.parent.gameObject.SetActive(false); // don't want to see my name/UI! Disable the canvas
        }

        rb.centerOfMass = new Vector3(0, centerOfMassHeight, 0);
        aS = GetComponent<AudioSource>();

        rearviewBorder = GameObject.Find("Rearview Border");
        rearviewImage = rearviewBorder.transform.GetChild(0).gameObject;

        rear_left_light = transform.Find("RearLeftLight").gameObject;
        rear_right_light = transform.Find("RearRightLight").gameObject;
    }

    public void Start()
    {
        // If not my car, set the canvas to my car's camera so that I can see the other car's UI (name)
        if (!PV.IsMine)
        {
            transform.Find("Canvas").GetComponent<Canvas>().worldCamera = GameManager.instance.onwPlayer.transform.Find("CameraHolder").Find("Camera").GetComponent<Camera>();
        }

    }

    void OnValidate()
    {
        rb.centerOfMass = new Vector3(0, centerOfMassHeight, 0);
    }

    void Update ()
    {
        if(Input.GetKeyDown(KeyCode.C) && PV.IsMine)
        {
            rearviewImage.SetActive(!rearviewImage.activeSelf);
            rearviewBorder.SetActive(!rearviewBorder.activeSelf);
        }
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
        BrakeLights();
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

    void BrakeLights()
    {
        if(isBreaking)
        {
            rear_left_light.SetActive(true);
            rear_right_light.SetActive(true);
        }
        else
        {
            rear_left_light.SetActive(false);
            rear_right_light.SetActive(false);
        }
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
        if (stream.IsWriting && PV.IsMine)
        {
            // We own this player: send the others our data
            stream.SendNext(aS.volume);
            stream.SendNext(aS.pitch);
            stream.SendNext(rear_left_light.activeSelf);
            stream.SendNext(rear_right_light.activeSelf);
        }
        else if (stream.IsWriting == false && PV.IsMine == false)
        {
            // Network player, receive data
            aS.volume = (float)stream.ReceiveNext();
            aS.pitch = (float)stream.ReceiveNext();
            rear_left_light.SetActive((bool)stream.ReceiveNext());
            rear_right_light.SetActive((bool)stream.ReceiveNext());
        }
    }

}