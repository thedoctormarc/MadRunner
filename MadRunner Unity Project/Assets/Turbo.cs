using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turbo : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        active = true;
        rend = GetComponent<MeshRenderer>();
        aS = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(active == false)
        {
            if((currentReloadTime += Time.deltaTime) >= reloadTime)
            {
                currentReloadTime = 0f;
                SetActive(true);
            }
        }
    }

    public void SetActive(bool active)
    {
       if (active == false)
        {
            aS.Play();
        }

        this.active = active;
        rend.enabled = active;
        transform.GetChild(0).gameObject.SetActive(active);
    }
}
