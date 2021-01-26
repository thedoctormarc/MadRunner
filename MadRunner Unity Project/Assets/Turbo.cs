using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turbo : MonoBehaviour
{

    [SerializeField]
    [Range(1f, 3f)]
    public float pickupTurbo;

    [SerializeField]
    [Range(5f, 20f)]
    public float reloadTime = 10f;

    public float currentReloadTime = 0f;

    public bool active;
    MeshRenderer rend;

    // Start is called before the first frame update
    void Start()
    {
        active = true;
        rend = GetComponent<MeshRenderer>();
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
        this.active = active;
        rend.enabled = active;
        transform.GetChild(0).gameObject.SetActive(active);
    }
}
