using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.HID;

public class MoveController : MonoBehaviour
{
    public LookController lookCon;
    public PlayerInputInfo inputInfo;
    public CharacterController cc;
    public Animator playerAnimator;

    public float moveSpeed = 5f;
    public float moveSharpness = 12f;

    public float jumpForce;
    public float gravityForce;
    public float airaccelerationSpeed;
    public float drag;
    public LayerMask groundCheckLayerMask;
   


    const float SWEEPTEST_BIAS = 0.02f;

    [SerializeField] AnimationCurve _landEffectCurve;
    [SerializeField] Transform _cameraPosition;
    [SerializeField] float _maxLandHeightTime = 0.15f;


    Vector3 _groundNormal;

    RaycastHit _hit;
    
    bool _isGrounded = false;
    bool _wasGrounded;
    bool _foundAnyGround;

    float _lastTimeJumped;
    float _inAirTime;
    float _inputSmooth;
    float _latestImpactSpeed;
    float _landEffectY;
    float _lastLandEffectY;
    float _landEffectCurveTime;
    float _landEffectAmount;

    public Vector3 CurrentVelocity { get; private set; }

    public UnityAction<float> onPlayerLanded;

    private void Start()
    {
        GameManager.instance.playerHit = GetComponent<HitableObj>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale <= 0.0f) return;

        //회전설정
        transform.rotation = lookCon.CamHolder.rotation;
        _wasGrounded = _isGrounded;
        GroundCheck();

        var fallspeed = -Mathf.Min(CurrentVelocity.y, _latestImpactSpeed);
        fallspeed = Mathf.Clamp(fallspeed, 0.02f, 15f);

        if(!_wasGrounded && _isGrounded)
        {
            onPlayerLanded?.Invoke(fallspeed *2f);
            StartLandEffect(fallspeed);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            AddVelocity(Vector3.up * 15f);
        }

        HandleMovement();

        UpdateCameraYPosition();

        _inputSmooth = Mathf.Lerp(_inputSmooth, (cc.velocity.magnitude / moveSpeed) * inputInfo.Move.magnitude * (_isGrounded ? 1f : 0f), 8f * Time.deltaTime);

        playerAnimator.SetFloat("MoveMotion", _inputSmooth);
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
                    && !(!_wasGrounded && (_hit.distance - SWEEPTEST_BIAS) > 0.07f))
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
        if(GameManager.instance.state == GameManager.GameState.Ready)
        {
            currentMoveInput = Vector3.zero;
        }

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
                CurrentVelocity += velocityDiff * (airaccelerationSpeed * Time.deltaTime);
            }

            CurrentVelocity += Vector3.down * (gravityForce * Time.deltaTime);


            CurrentVelocity *= (1f / (1f + (drag * Time.deltaTime)));

            if (CurrentVelocity.y < -1f) onPlayerLanded?.Invoke(Mathf.Max(CurrentVelocity.y * 0.05f, -0.25f));
            else if (CurrentVelocity.y > 1f) onPlayerLanded?.Invoke(Mathf.Min(CurrentVelocity.y * 0.02f, 0.1f));
        }

        if (_inAirTime <= 0.072f && _lastTimeJumped == 0f)
        {
            if (inputInfo.Jump)
            {
                onPlayerLanded?.Invoke(-7f);
                CurrentVelocity += (Vector3.up * jumpForce) - Vector3.Project(CurrentVelocity, Vector3.up);
                _lastTimeJumped = Time.time;
            }
        }

        var lastBottomHemiSphere = GetBottomHemiSphere();
        var lastTopHemiSphere = GetTopHemiSphere();

        cc.Move(CurrentVelocity * Time.deltaTime);

        _latestImpactSpeed = 0f;

        if (!_wasGrounded &&
            Physics.CapsuleCast(lastBottomHemiSphere, lastTopHemiSphere, cc.radius,
            CurrentVelocity.normalized, out RaycastHit hit, CurrentVelocity.magnitude * Time.deltaTime, -1,
            QueryTriggerInteraction.Ignore))
        {
            _latestImpactSpeed = CurrentVelocity.y;
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

    public void AddVelocity(Vector3 force, bool isJump = true)
    {
        if (isJump)
        {
            onPlayerLanded?.Invoke(-15f);
            _lastTimeJumped = Time.time;
        }
        CurrentVelocity += force;
    }

    public Vector3 GetBottomHemiSphere()
    {
        return transform.position + Vector3.up * (cc.radius);
    }
    public Vector3 GetTopHemiSphere()
    {
        return transform.position + Vector3.up * (cc.height - cc.radius);
    }

    public void StartLandEffect(float landForce)
    {
        _lastLandEffectY = _landEffectY;
        _landEffectCurveTime = 0f;
        _landEffectAmount = Mathf.Min(landForce, 15f);
    }

    private void UpdateCameraYPosition()
    {
        var maxYvalue = _landEffectCurve.Evaluate(_maxLandHeightTime);

        if (_landEffectCurveTime > _maxLandHeightTime && _lastLandEffectY < 0f)
        {
            var currentLandEffectY = maxYvalue * _landEffectAmount + _lastLandEffectY;
            _lastLandEffectY = 0f;
            _landEffectAmount = Mathf.Min(currentLandEffectY / maxYvalue, 15f);
        }

        _landEffectY = _landEffectCurve.Evaluate(_landEffectCurveTime) * _landEffectAmount;
        _landEffectCurveTime += Time.deltaTime;

        var finalLandEffectY = Mathf.Max(maxYvalue * 15f, _lastLandEffectY + _landEffectY);

        _cameraPosition.localPosition = new Vector3(0f, 1.65f + finalLandEffectY, 0f);
    }
}
