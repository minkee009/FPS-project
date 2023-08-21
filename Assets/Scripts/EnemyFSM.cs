using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.XR;

// 목표 : 적을 FSM 다이어그램에 따라 동작시키고 싶다.
// 필요 속성 : 적 상태,상태 기계

// 목표2 : 플레이어와의 거리를 측정한 뒤 특정 상태로 만들어준다.
// 필요 속성 : 플레이어와의 거리
public class EnemyFSM : MonoBehaviour
{
    public enum EnemyState { Idle = 0, Move, Attack, Return, Damaged, Die }

    public EnemyState CurrentState { get; private set; }

    public CharacterController cc;

    public HitableObj hitableObj;

    Transform _playerTransform;
    public float findDist = 5f;
    public float returnDist = 20f;
    public float moveSpeed = 4f;
    public float drag = 0.1f;
    public float airAcceleration = 4f;
    public float gravityForce = 20f;
    public float jumpForce = 8f;

    public float attackDist = 3f;
    public float attackDelay = 2f;

    bool isAttacked = false;
    Vector3 _currentVelocity;
    Vector3 _targetVelocity;
    Vector3 _originPos;

    // Start is called before the first frame update
    void Start()
    {
        CurrentState = EnemyState.Idle;
        _originPos = new Vector3(transform.position.x,0f,transform.position.z);
        hitableObj = GetComponent<HitableObj>();
        hitableObj.OnHit += DamageAction;
        cc = GetComponent<CharacterController>();
        _playerTransform = GameObject.Find("Actor").transform;
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentState)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Return:
                Return();
                break;
            case EnemyState.Damaged:
                _targetVelocity = Vector3.zero;
                break;
            case EnemyState.Die:
                _targetVelocity = Vector3.zero;
                break;
        }

        if (!cc.isGrounded)
        {
            if(_targetVelocity.sqrMagnitude > 0f)
            {
                Vector3 velocityDiff = Vector3.ProjectOnPlane(_targetVelocity - _currentVelocity, Vector3.down * 20f);
                _currentVelocity += velocityDiff * airAcceleration * Time.deltaTime;
            }
            _currentVelocity *= (1f / (1f + (drag * Time.deltaTime)));
            _currentVelocity += Vector3.down * gravityForce * Time.deltaTime;
        }
        else
        {
            _currentVelocity = Vector3.Lerp(_currentVelocity, _targetVelocity, 12f * Time.deltaTime);
        }

        var lastBottomHemiSphere = GetBottomHemiSphere();
        var lastTopHemiSphere = GetTopHemiSphere();

        cc.Move(_currentVelocity * Time.deltaTime);

        if (!cc.isGrounded &&
            Physics.CapsuleCast(lastBottomHemiSphere, lastTopHemiSphere, cc.radius,
            _currentVelocity.normalized, out RaycastHit hit, _currentVelocity.magnitude * Time.deltaTime, -1,
            QueryTriggerInteraction.Ignore))
        {
            _currentVelocity = Vector3.ProjectOnPlane(_currentVelocity, hit.normal);
        }
    }

    private void Idle()
    {
        _targetVelocity = Vector3.zero;

        var nonYDir = (_playerTransform.position - transform.position);
        nonYDir = new Vector3(nonYDir.x, 0f, nonYDir.z);

        if(nonYDir.magnitude < findDist)
        {
            CurrentState = EnemyState.Move;
        }
    }

    private void Move()
    {
        isAttacked = false;
        var dir = (_playerTransform.position - transform.position);
        var nonYDir = new Vector3(dir.x, 0f, dir.z).normalized;
        _targetVelocity = nonYDir * moveSpeed;
        
        if(new Vector3(dir.x,0f,dir.z).magnitude < attackDist)
        {
            CurrentState = EnemyState.Attack;
        }

        var nonYPos = new Vector3(transform.position.x, 0f, transform.position.z);

        if((nonYPos - _originPos).magnitude > returnDist)
        {
            CurrentState = EnemyState.Return;
        }
    }

    float attackTimer = 0f;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(_validAttack && hit.gameObject.TryGetComponent(out HitableObj hitObj))
        {
            _validAttack = false;
            hitObj.Hit(-1, gameObject, false);
        }
        if (_validAttack && cc.isGrounded)
        {
            _validAttack = false;
        }
    }

    private void Attack()
    {
        attackTimer += Time.deltaTime;

        if(attackTimer > attackDelay)
        {
            attackTimer = 0f;
            CurrentState = EnemyState.Move;
            return;
        }

        _targetVelocity = Vector3.zero;

        if (!isAttacked && cc.isGrounded)
        {
            var dir = (_playerTransform.position - transform.position);
            var nonYDir = new Vector3(dir.x, 0f, dir.z).normalized;
            _currentVelocity = nonYDir * moveSpeed;
            isAttacked = true;
            _validAttack = true;
            _currentVelocity += Vector3.up * jumpForce;
        }
    }

    bool _validAttack = false;

    private void Return()
    {
        var nonYPos = new Vector3(transform.position.x, 0f, transform.position.z);
        var nonYDir = (_originPos - nonYPos).normalized;

        var toPlayerDir = (_playerTransform.position - transform.position);

        _targetVelocity = nonYDir * moveSpeed;

        if ((_originPos - nonYPos).magnitude < 0.1f)
        {
            CurrentState = EnemyState.Idle;
        }
    }
     
    public void DamageAction()
    {
        if(hitableObj.Hp <= 1)
        {
            CurrentState = EnemyState.Die;
            Die();
            return;
        }
        if (CurrentState == EnemyState.Damaged)
            return;

        if(hitableObj.Hp > 1)
        {
            CurrentState = EnemyState.Damaged;
            Damaged();
        }
        
    }


    private void Damaged()
    {
        //피격 모션 0.5

        //피격 상태 처리를 위한 코루틴 실행
        StartCoroutine(DamageProcess());
    }

    IEnumerator DamageProcess()
    {
        yield return new WaitForSeconds(0.5f);
        CurrentState = EnemyState.Move;
    }

    IEnumerator DieProcess()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    private void Die()
    {
        _targetVelocity = Vector3.zero;
        StopAllCoroutines();
        StartCoroutine(DieProcess());
    }


    //////////////////////////////-----------------------------                         ------------------------------//////////////////////////////////


    public Vector3 GetBottomHemiSphere()
    {
        return transform.position + Vector3.up * (cc.radius);
    }
    public Vector3 GetTopHemiSphere()
    {
        return transform.position + Vector3.up * (cc.height - cc.radius);
    }

    private void OnDestroy()
    {
        hitableObj.OnHit -= DamageAction;
    }
}
