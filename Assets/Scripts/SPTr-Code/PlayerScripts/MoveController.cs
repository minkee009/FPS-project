using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class MoveController : MonoBehaviour
{
    public LookController lookCon;
    public PlayerInputInfo inputInfo;
    public CharacterController cc;

    public float moveSpeed = 5f;
    public float moveSharpness = 12f;

    public float jumpForce;
    public float gravityForce;
    public float airaccelerationSpeed;
    public float drag;
    public LayerMask groundCheckLayerMask;

    const float SWEEPTEST_BIAS = 0.02f;

    RaycastHit _hit;
    bool _isGrounded = false;
    bool _wasGrounded;
    bool _foundAnyGround;
    Vector3 _groundNormal;
    float _lastTimeJumped;
    float _inAirTime;

    public Vector3 CurrentVelocity { get; private set; }

    // Update is called once per frame
    void Update()
    {
        //회전설정
        transform.rotation = lookCon.CamHolder.rotation;
        _wasGrounded = _isGrounded;
        GroundCheck();
        HandleMovement();
    }

    void GroundCheck()
    {
        var checkDist = _isGrounded ? (cc.skinWidth + 1f) : 0.07f;
        _groundNormal = Vector3.up;
        _isGrounded = false;
        _foundAnyGround = false;
        if (Time.time >= _lastTimeJumped + 0.2f)
        {
            if (Physics.SphereCast(GetBottomHemiSphere(), cc.radius, Vector3.down, out _hit, checkDist + SWEEPTEST_BIAS, groundCheckLayerMask, QueryTriggerInteraction.Ignore))
            {
                _groundNormal = _hit.normal;
                _foundAnyGround = true;
                if (Mathf.Acos(Vector3.Dot(_hit.normal, Vector3.up)) * Mathf.Rad2Deg < cc.slopeLimit
                    && !(!_wasGrounded && _hit.distance > 0.07f))
                {
                    _isGrounded = true;
                    _lastTimeJumped = 0f;
                    _inAirTime = 0f;
                    if (_hit.distance > cc.skinWidth && cc.enabled)
                    {
                        cc.Move(Vector3.down * (_hit.distance - SWEEPTEST_BIAS));
                    }
                    return;
                }
                
            }
        }
        
        _inAirTime += Time.deltaTime;
    }

    void HandleMovement()
    {
        var targetVelocity = Vector3.zero;
        var currentMoveInput = MoveStateExtensions.ComputeMoveVector(inputInfo.Move, lookCon);
        if (_isGrounded)
        {
            var inputRight = Vector3.Cross(currentMoveInput, Vector3.up);
            var reorientedInput = Vector3.Cross(_groundNormal, inputRight).normalized * currentMoveInput.magnitude;
            targetVelocity = reorientedInput * moveSpeed;
            CurrentVelocity = Vector3.Lerp(CurrentVelocity, targetVelocity, 12f * Time.deltaTime);
        }
        else
        {
            if (currentMoveInput.sqrMagnitude > 0f)
            {
                targetVelocity = currentMoveInput * moveSpeed;

                if (_foundAnyGround)
                {
                    Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Vector3.up, _groundNormal), Vector3.up).normalized;
                    targetVelocity = Vector3.ProjectOnPlane(targetVelocity, perpenticularObstructionNormal);
                }

                Vector3 velocityDiff = Vector3.ProjectOnPlane(targetVelocity - CurrentVelocity, Vector3.down * gravityForce);
                CurrentVelocity += velocityDiff * airaccelerationSpeed * Time.deltaTime;
            }

            CurrentVelocity += Vector3.down * gravityForce * Time.deltaTime;


            CurrentVelocity *= (1f / (1f + (drag * Time.deltaTime)));
        }

        if (_inAirTime <= 0.072f && _lastTimeJumped == 0f)
        {
            if (inputInfo.Jump)
            {
                CurrentVelocity += (Vector3.up * jumpForce) - Vector3.Project(CurrentVelocity, Vector3.up);
                _lastTimeJumped = Time.time;
            }
        }

        var lastBottomHemiSphere = GetBottomHemiSphere();
        var lastTopHemiSphere = GetTopHemiSphere();

        cc.Move(CurrentVelocity * Time.deltaTime);

        if (!_isGrounded &&
            Physics.CapsuleCast(lastBottomHemiSphere, lastTopHemiSphere, cc.radius,
            CurrentVelocity.normalized, out RaycastHit hit, CurrentVelocity.magnitude * Time.deltaTime, -1,
            QueryTriggerInteraction.Ignore))
        {
            CurrentVelocity = Vector3.ProjectOnPlane(CurrentVelocity, hit.normal);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + Vector3.up * cc.radius, cc.radius);

        var checkDist = _isGrounded ? (cc.skinWidth + 1f) : 0.07f;
        Physics.SphereCast(GetBottomHemiSphere(), cc.radius, Vector3.down, out _hit, checkDist + SWEEPTEST_BIAS, groundCheckLayerMask, QueryTriggerInteraction.Ignore);

        Gizmos.DrawWireSphere(GetBottomHemiSphere() + Vector3.down * (_hit.distance - SWEEPTEST_BIAS), cc.radius);
    }

    public Vector3 GetBottomHemiSphere()
    {
        return transform.position + Vector3.up * (cc.radius);
    }
    public Vector3 GetTopHemiSphere()
    {
        return transform.position + Vector3.up * (cc.height - cc.radius);
    }
}
