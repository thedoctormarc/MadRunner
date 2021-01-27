using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Realtime;
using Photon.Pun;

public class Turbo : MonoBehaviourPunCallbacks
{

    [SerializeField]
    [Range(1f, 3f)]
    public float pickupTurbo = 1f;

    [SerializeField]
    [Range(5f, 20f)]
    public float reloadTime = 10f;

    float currentReloadTime = 0f;

    [HideInInspector]
    public bool active;
    MeshRenderer rend;
    AudioSource aS;

    PhotonView PV;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        active = true;
        rend = GetComponent<MeshRenderer>();
        aS = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.IsMasterClient == true)
        {
            if (active == false)
            {
                if ((currentReloadTime += Time.deltaTime) >= reloadTime)
                {
                    currentReloadTime = 0f;
                    InvokeSetActive(true);
                }
            }
        }

    }

   public void InvokeSetActive(bool active)
    {
        SetActive(active);
        PV.RPC("SetActive", RpcTarget.Others, active, false);
    }

    [PunRPC]
    public void SetActive(bool active, bool playAudio = true)
    {
       if (active == false && playAudio == true)
        {
            aS.Play();
        }

        this.active = active;
        rend.enabled = active;
        transform.GetChild(0).gameObject.SetActive(active);
    }
}
