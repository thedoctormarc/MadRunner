using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float mouse_sensitivity, sprint_speed, walk_speed, jump_force, smooth_time;

    float vertical_look_rotation;
    bool grounded;
    Vector3 smooth_move_velocity;
    Vector3 move_amount;

    Rigidbody rb;
    PhotonView PV;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }

        // TODO: This is for testing purposes only. Here we test the rotation of the car with the mouse.
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouse_sensitivity);
    }
}
