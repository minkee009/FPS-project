using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAction : MonoBehaviour
{
    public float bombTimer;
    public GameObject bombEffect;
    public LayerMask dmgMask;

    Collider[] _dmgCols;

    private void Start()
    {
        _dmgCols = new Collider[15];
    }

    private void Update()
    {
        bombTimer += Time.deltaTime;
        if(bombTimer > 3f)
        {
            Explosion();
        }
    }

    public void Explosion()
    {
        var count = Physics.OverlapSphereNonAlloc(transform.position, 5f, _dmgCols, dmgMask, QueryTriggerInteraction.Ignore);

        if(count > 0)
        {
            Debug.Log(count);
            

            for (int i = 0; i < count; i++)
            {
                var toVector = _dmgCols[i].transform.position - transform.position;

                var isHit = Physics.Raycast(new Ray(transform.position, toVector.normalized), out RaycastHit hitInfo, toVector.magnitude, -1, QueryTriggerInteraction.Ignore);

                if (!isHit || (isHit && hitInfo.collider == _dmgCols[i]))
                {

                    Debug.Log(_dmgCols[i].name);    
                    if (_dmgCols[i].TryGetComponent(out HitableObj hitObj)) 
                    {
                        hitObj.Hit(-5f, gameObject, false);
                    }
                   
                    if (_dmgCols[i].TryGetComponent(out MoveController move))
                    {
                        move.AddVelocity(toVector.normalized * 15f + Vector3.up * 20f);
                    }

                    if (_dmgCols[i].TryGetComponent(out EnemyFSM eFSM))
                    {
                        eFSM.AddVelocity(toVector.normalized * 15f + Vector3.up * 20f);
                    }
                    if (_dmgCols[i].TryGetComponent(out Rigidbody rb) && !rb.isKinematic)
                    {
                        rb.AddForce(toVector.normalized * 15f + Vector3.up * 10f, ForceMode.Impulse);
                    }
                }
                
            }
        }

        var bombEffectGO = Instantiate(bombEffect);
        bombEffectGO.transform.position = transform.position;

        Destroy(gameObject);
    }
}
