using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFire : MonoBehaviour
{
    public PlayerInputInfo inputInfo;
    public GameObject bomb;
    public GameObject firePosition;
    public GameObject hitEffect;
    public LookController lookCon;
    public Transform muzzlePos;

    public GameObject[] muzzleFlashEffects;

    public Animator playerAnimator;

    public LayerMask bulletRayLayerMask;

    ParticleSystem _particleSys;
    RaycastHit _hitInfo;

    private void Start()
    {
        _particleSys = hitEffect.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.state != GameManager.GameState.Start
            || Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }
        if (inputInfo.UseItem || Input.GetMouseButtonDown(2))
        {
            var bombGO = Instantiate(bomb);
            bombGO.transform.position = transform.position;

            Rigidbody rb = bombGO.GetComponent<Rigidbody>();

            rb.MovePosition(bombGO.transform.position);
            rb.AddForce(transform.forward * 12f, ForceMode.VelocityChange);
        }

        var hitPosition = transform.parent.position + transform.parent.forward * 60f;
        var bulletRay = new Ray(transform.parent.position, transform.parent.forward);

        if (Physics.Raycast(bulletRay, out _hitInfo, Mathf.Infinity, bulletRayLayerMask, QueryTriggerInteraction.Ignore))
        {
            hitPosition = _hitInfo.point;
        }

        if (inputInfo.Attack)
        {
            playerAnimator.SetTrigger("Shoot");
            lookCon.AddViewPunch(Vector3.right * -4f + Vector3.up * UnityEngine.Random.Range(-3f, 3f));

            var effect = Instantiate(muzzleFlashEffects[Random.Range(0, muzzleFlashEffects.Length - 1)],muzzlePos);
            effect.transform.position = muzzlePos.position;
            effect.transform.rotation = muzzlePos.rotation;
            //playerAnimator.ResetTrigger("Shoot");
        }

        if (inputInfo.Attack && _hitInfo.transform != null)
        {
            //Debug.Log(_hitInfo.transform.name);

            if(_hitInfo.transform.TryGetComponent(out Rigidbody rb))
            {
                rb.AddForceAtPosition(transform.parent.forward * 5f, _hitInfo.point,ForceMode.Impulse);
            }

            hitEffect.transform.position = _hitInfo.point;
            hitEffect.transform.forward = _hitInfo.normal;
            _particleSys.Play();

            if(_hitInfo.transform.TryGetComponent(out HitableObj hitObj))
            {
                hitObj.Hit(-1,gameObject,false);
            }
        }

        Debug.DrawRay(firePosition.transform.position, hitPosition - firePosition.transform.position, Color.red);
    }
}
