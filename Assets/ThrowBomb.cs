using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowBomb : MonoBehaviour
{
    public PlayerInputInfo inputInfo;
    public Transform camTransform;
    public GameObject bomb;
    public GameObject firePosition;

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            var bombGO = Instantiate(bomb);
            bombGO.transform.position = firePosition.transform.position;

            Rigidbody rb = bombGO.GetComponent<Rigidbody>();

            rb.AddForce(camTransform.forward * 12f, ForceMode.VelocityChange);
        }
    }
}
