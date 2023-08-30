using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadLightOnOff : MonoBehaviour
{
    public Light headLight;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            headLight.enabled = !headLight.enabled;
        }
    }
}
