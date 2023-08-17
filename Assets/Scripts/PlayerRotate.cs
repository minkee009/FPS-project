using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public PlayerInputInfo inputInfo;
    //public float speed;
    public Transform camTransfrom;

    float lookX = 0;
    float lookY = 0;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        lookX += inputInfo.Look.x;
        lookY -= inputInfo.Look.y;

        lookY = Mathf.Clamp(lookY, -90f, 90f);

        camTransfrom.rotation = Quaternion.Euler(lookY, lookX, 0);
    }
}
