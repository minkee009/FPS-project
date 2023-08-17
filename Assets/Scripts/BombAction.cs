using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAction : MonoBehaviour
{
    public GameObject BombEffect;

    private void OnCollisionEnter(Collision collision)
    {
        var bombEffectGO = Instantiate(BombEffect);
        bombEffectGO.transform.position = transform.position;

        Destroy(gameObject);
    }
}
