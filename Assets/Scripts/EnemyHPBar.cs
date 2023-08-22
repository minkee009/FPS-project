using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHPBar : MonoBehaviour
{
    public Transform targetT;

    // Update is called once per frame
    void Update()
    {
        transform.forward = targetT.forward * -1f;
    }
}
