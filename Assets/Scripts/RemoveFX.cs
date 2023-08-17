using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveFX : MonoBehaviour
{
    public float duration = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,duration);
    }
}
