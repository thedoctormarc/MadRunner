using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class CarController : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IPunObservable
{
    private float horizontalInput;
    private float verticalInput;
    private float steerAngle;
    private bool isBreaking;
    private bool isTurbo;
    private bool isGrounded = true;
    List<Collider> slipStreams;

    // for interpolation
    private bool isTurboPressedDown;
    private bool isTurboReleased;

    private bool needsReset = false;

    public WheelCollider LF_Col, RF_Col, LB_Col, RB_Col;
    public Transform LF, RF, LB, RB;

    public float maxSteeringAngle = 30f;
    public float motorForce = 50f;
    public float brakeForce = 0f;

    [Range(2.0f, 5.0f)]
    public float maxBrakeSpeed = 5.0f;

    public Rigidbody rb;
    public float rbVelocity = 0f;

    [Range(0.25f, 2.0f)]
    public float centerOfMassHeight = 0.25f;

    private AudioSource aS;

    PhotonView PV;

    public GameObject rearviewImage, rearviewBorder;

    // Lap control
    GameObject score;
    ScoreLogic score_logic;

    // Lights
    GameObject rear_left_light;
    GameObject rear_right_light;

    // Turbo
    GameObject left_turbo;
    GameObject right_turbo;

    float t = 0.0f;
    bool camera_turbo_change = false;

    private float max_fov = 90.0f;
    private float default_fov = 60.0f;

    public Camera cam; // Car's main camera

    // UI
    GameObject playerNameText;

    // PP reference
    GameObject pp;
    PostProcessVolume pp_v;
    ChromaticAberration ca;

    // to simulate surfaces
    float _angularDrag;
    float _sidewaysStifness;

    [SerializeField]
    [Range(3f, 6f)]
    float slipStreamStrength = 4f;
    [SerializeField]
    [Range(45f, 90f)]
    float maxSlipStreamAngle = 90f;
    [SerializeField]
    [Range(1.1f, 1.34f)]
    float slipStreamVelocityExp = 1.23f;
    [SerializeField]
    float approxTopSpeedWithSlipStream = 75f;

    // Slip particles
    ParticleSystem slipStreamP;

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
            Destroy(GetComponent<AudioListener>());
        }
        else
        {
            GameManager.instance.onwPlayer = gameObject;
            playerNameText.transform.parent.gameObject.SetActive(false); // don't want to see my name/UI! Disable the canvas
            slipStreams = new List<Collider>();
            slipStreamP = transform.Find("SlipstreamParticles").GetComponent<ParticleSystem>();
            SetSlipStreamAlpha(0f);
        }

        rb.centerOfMass = new Vector3(0, centerOfMassHeight, 0);

        // get rb values
        _angularDrag = rb.angularDrag;
        _sidewaysStifness = transform.Find("Wheels").Find("Colliders").GetChild(0).GetComponent<WheelCollider>().sidewaysFriction.stiffness;

        score = GameObject.Find("Score");
        score_logic = score.GetComponent<ScoreLogic>();

        aS = GetComponent<AudioSource>();

        rearviewBorder = GameObject.Find("Rearview Border");
        rearviewImage = rearviewBorder.transform.GetChild(0).gameObject;

        rear_left_light = transform.Find("RearLeftLight").gameObject;
        rear_right_light = transform.Find("RearRightLight").gameObject;

        left_turbo = transform.Find("LeftTurbo").gameObject;
        right_turbo = transform.Find("RightTurbo").gameObject;

        pp = GameObject.Find("PostProcessingEffects");
        pp_v = pp.GetComponent<PostProcessVolume>();
        ca = pp_v.profile.GetSetting<ChromaticAberration>();

        
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
        if(PV && PV.IsMine)
        {
            rb.centerOfMass = new Vector3(0, centerOfMassHeight, 0);
        }
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
        CheckTurbo();
        AddSlipStream();
        UpdateWheels();
        AdjustAudio();
        BrakeLights();
        CheckReset();
    }

    private void AddSlipStream()
    {
        if(slipStreams.Count == 0)
        {
            SetSlipStreamAlpha(0f);
            return;
        }

        float factor = 0f;

        foreach(Collider col in slipStreams)
        {
            float colFactor = 0f;
            GameObject otherCar = col.transform.parent.gameObject;
            Vector3 otherCarPos = otherCar.transform.position;
            Vector3 otherCarDir = otherCar.transform.forward;
            float dist = (transform.position - otherCarPos).magnitude;
            float rotDiff = Vector3.Angle(transform.forward, otherCarDir);
            if(rotDiff >= maxSlipStreamAngle)
            {
                continue;
            }


            BoxCollider otherCarCollider = otherCar.GetComponent<BoxCollider>();
            float maxDist = col.bounds.size.z + (otherCarCollider.bounds.size.z / 2f);
            float minDist = otherCarCollider.bounds.size.z / 2f;

            if (dist > maxDist)
            {
                dist = maxDist;
            }

            // NewValue = (((OldValue - OldMin) * (NewMax - NewMin)) / (OldMax - OldMin)) + NewMin
            float distFactorNormalized = (((dist - minDist) * (1f - 0f)) / (maxDist - minDist)) + 0f;
            distFactorNormalized = 1f - distFactorNormalized;

            float angleFactorNormalized = (((rotDiff - 0f) * (1f - 0f)) / (maxSlipStreamAngle - 0f)) + 0f;
            angleFactorNormalized = 1f - angleFactorNormalized;

            float otherVel = otherCar.GetComponent<CarController>().rbVelocity;
            float velocitySq = Mathf.Pow(rb.velocity.magnitude, slipStreamVelocityExp);
            float otherVelocitySq = Mathf.Pow(otherVel, slipStreamVelocityExp);
            float velocitiesSq = velocitySq + otherVelocitySq;
            float maxVelocitiesSq = Mathf.Pow(approxTopSpeedWithSlipStream, slipStreamVelocityExp) * 2f;
            float velocityFactorNormalized = (((velocitiesSq - 0f) * (1f - 0f)) / (maxVelocitiesSq - 0f)) + 0f;
             

            colFactor += distFactorNormalized / 3f;
            colFactor += angleFactorNormalized / 3f;
            colFactor += velocityFactorNormalized / 3f;

            Debug.Log("Slisptream distance factor: " + distFactorNormalized + ", angle factor:" + angleFactorNormalized + "and speed factor:" + velocityFactorNormalized);

            factor += colFactor;
        }

        Mathf.Clamp(factor, 0f, 1f);

        SetSlipStreamAlpha(factor);

        if (factor == 0f)
        {
            return;
        }

        rb.AddForce(transform.forward * factor * slipStreamStrength, ForceMode.Acceleration);

        Debug.Log("Adding slipstream force to car with a magnitude of: " + (transform.forward * factor * slipStreamStrength).magnitude);
    }

    private void GetInput()
    {
        if(score_logic.started)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            isBreaking = Input.GetKey(KeyCode.Space);
            isTurbo = Input.GetKey(KeyCode.LeftShift) && isGrounded;
            isTurboReleased = Input.GetKeyUp(KeyCode.LeftShift);
            isTurboPressedDown = Input.GetKeyDown(KeyCode.LeftShift);
            needsReset = Input.GetKeyDown(KeyCode.R);
        }
    }

    private void Steering()
    {
        steerAngle = maxSteeringAngle * horizontalInput;
        LF_Col.steerAngle = steerAngle;
        RF_Col.steerAngle = steerAngle;
    }

    private void Motor()
    {
        rbVelocity = rb.velocity.magnitude;

        LF_Col.motorTorque = verticalInput * motorForce;
        RF_Col.motorTorque = verticalInput * motorForce;

        float bForce = (isBreaking) ? brakeForce : 0f;

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
        aS.volume  =  Math.Max(0.2f, (((rb.velocity.magnitude - 0f) * (1f - 0f)) / (approxTopSpeedWithSlipStream - 0f)) + 0f); // volume depends on speed
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

    void CheckTurbo()
    {
        if (isTurbo)
        {
            ca.intensity.value = 1.0f;

            if(!camera_turbo_change)
            {
                t = 0.0f;
                camera_turbo_change = true;
            }
            else if(t < 1.0f)
            {
                cam.fieldOfView = Mathf.Lerp(default_fov, max_fov, t);
                t += Time.deltaTime * 3.0f;
            }

            left_turbo.SetActive(true);
            right_turbo.SetActive(true);

            rb.AddForce(transform.forward * 3.5f, ForceMode.Acceleration);
        }
        else
        {
            ca.intensity.value = 0.0f;

            if (camera_turbo_change)
            {
                t = 0.0f;
                camera_turbo_change = false;
            }
            else if(t < 1.0f)
            {
                cam.fieldOfView = Mathf.Lerp(max_fov, default_fov, t);
                t += Time.deltaTime * 3.0f;
            }

            left_turbo.SetActive(false);
            right_turbo.SetActive(false);
        }
    }

    void CheckReset()
    {
        if(needsReset)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 10.0f, transform.position.z);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0.0f);

            needsReset = false;
        }
    }

    void SetSlipStreamAlpha(float normalizedFactor)
    {
        if (slipStreamP == null)
        {
            return;
        }

    //    Debug.Log("Setting the slipstream alpha to: " + normalizedFactor);
        ParticleSystem.MainModule pModule = slipStreamP.main;
        Color newColor = pModule.startColor.color;
        newColor.a = normalizedFactor;
        pModule.startColor = newColor;

    }

    void OnCollisionEnter(Collision collision) // if colliding with a dynamic prop, transfer ownership from master client to our client, so they can interact  
    {
        if (this.PV.IsMine)
        {
            switch (collision.gameObject.tag)
            {
             
                case "ground":
                    {
                        isGrounded = true;
                        break;
                    }
            }
            return;
        }

        PhotonView PV = collision.gameObject.GetComponent<PhotonView>();
        CarController CC = collision.gameObject.GetComponent<CarController>();

        if (PV && !CC)
        {
            Debug.Log("Car taking ownership of object from master so it can interact with it!");
            PV.TransferOwnership(PhotonNetwork.LocalPlayer);
        }
 
    }

    void OnTriggerEnter(Collider other)
    {
        PhotonView PV = other.gameObject.GetComponent<PhotonView>();
        CarController CC = other.gameObject.GetComponent<CarController>();

        if (PV && !CC)
        {
            Debug.Log("Car taking ownership of object from master so it can interact with it!");
            PV.TransferOwnership(PhotonNetwork.LocalPlayer);
        }

        if (this.PV.IsMine)
        {
            switch(other.gameObject.tag)
            {
                case "slipstream":
                    {
                        slipStreams.Add(other);
                        break;
                    }
              
                case "notground":
                    {
                        isGrounded = false;
                        break;
                    }
                case "water":
                    {
                        OnWater(true);
                        break;
                    }
            }
        return;
        }
  

    }

   void OnCollisionExit(Collision collision)
    {
        PhotonView PV = collision.gameObject.GetComponent<PhotonView>();
        CarController CC = collision.gameObject.GetComponent<CarController>();

        if (PV && !CC)
        {
            Debug.Log("Car taking ownership of object from master so it can interact with it!");
            PV.TransferOwnership(PhotonNetwork.LocalPlayer);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (this.PV.IsMine)
        {
            if (other.gameObject.CompareTag("slipstream") == true)
            {
                slipStreams.Remove(other);
                return;
            }
        }

        PhotonView PV = other.gameObject.GetComponent<PhotonView>();
        CarController CC = other.gameObject.GetComponent<CarController>();

        if (PV && !CC)
        {
            Debug.Log("Car taking ownership of object from master so it can interact with it!");
            PV.TransferOwnership(PhotonNetwork.LocalPlayer);

            if (other.gameObject.CompareTag("water") == true)
            {
                OnWater(false);
            }
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
            stream.SendNext(left_turbo.activeSelf);
            stream.SendNext(right_turbo.activeSelf);
            stream.SendNext(rbVelocity);
            stream.SendNext(slipStreamP.main.startColor.color.a);
        }
        else if (stream.IsWriting == false && PV.IsMine == false)
        {
            // Network player, receive data
            aS.volume = (float)stream.ReceiveNext();
            aS.pitch = (float)stream.ReceiveNext();
            rear_left_light.SetActive((bool)stream.ReceiveNext());
            rear_right_light.SetActive((bool)stream.ReceiveNext());
            left_turbo.SetActive((bool)stream.ReceiveNext());
            right_turbo.SetActive((bool)stream.ReceiveNext());
            rbVelocity = (float)stream.ReceiveNext();
            SetSlipStreamAlpha((float)stream.ReceiveNext());
        }
    }

    void OnWater(bool on)
    {
        float angularDrag = (on) ? 0f : _angularDrag;
        float stiffness = (on) ? 0.5f : _sidewaysStifness;

        rb.angularDrag = angularDrag;

        GameObject wheelColliders = transform.Find("Wheels").Find("Colliders").gameObject;

        for(int i = 0; i < wheelColliders.transform.childCount; ++i)
        {
            WheelFrictionCurve c = wheelColliders.transform.GetChild(i).GetComponent<WheelCollider>().sidewaysFriction;
            c.stiffness = stiffness;
        }
    }

}