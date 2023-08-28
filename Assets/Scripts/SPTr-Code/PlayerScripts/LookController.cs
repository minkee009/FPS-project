using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
public class LookController : MonoBehaviour
{
    private float _maxVerticalAngle = 90f;
    private float _maxHorizontalAngle = 45f;
    private float _lerpClampHoriTime = 6f;
    private float _lerpClampVertTime = 6f;

    private float _maxSoftClampVertUp = -60f;
    private float _maxSoftClampVertDown = 30f;

    private bool _softClampVerticalAngle = false;
    private bool _clampHorizontalAngle = false;

    private float _setClampHori = 0f;
    private float _lerpHori = 0f;
    private float _clampHori, _clampVert;

    [SerializeField] private Camera _playerCam;
    [SerializeField] private PlayerInputInfo _inputInfo;
    [SerializeField] private Transform _followTransform;

    public UnityAction<float> onChangedFov;

    public Camera PlayerCam => _playerCam;

    private float _lastHoriValue;
    private float _lastVertValue;

    [Range(60f, 90f)]
    public float DefaultCamFOV = 75f;
    public float TargetCamFOV { private set; get; }
    public float CurrentCamFOV { private set; get; }
    public float CamFovLerpTime { private set; get; }
    private bool isFixedFovValue = false;

    public float changedHori { get; private set; }
    public float changedVert { get; private set; }

    //트랜스폼
    public Transform CamHolder { get; private set; }
    public Transform AltCamHolder { get; private set; }

    //뷰펀치
    Vector3 _vecPunchAngle;
    Vector3 _vecPunchAngleVel;
    Vector3 _currentLocalRot;


    public Vector3 PunchForce = new Vector3(25f, 0f, 0f);

    const float PUNCH_DAMPING = 9.0f;
    const float PUNCH_SPRING_CONSTANT = 65.0f;


    public void InitializeController()
    {
        CamHolder = this.transform;
        AltCamHolder = this.transform.GetChild(0);
        Cursor.lockState = CursorLockMode.Locked;
        CurrentCameraPosition = _followTransform.position;

        CurrentCamFOV = DefaultCamFOV;
        SetCameraFov(0f, 6f);
    }

    public void ResetCameraFov()
    {
        isFixedFovValue = false;
        TargetCamFOV = 0f;
        CamFovLerpTime = 6f;
        CurrentCamFOV = DefaultCamFOV;
    }

    public void SetCameraFov(float targetFov, float lerpTime, bool fixedValue = false)
    {
        isFixedFovValue = fixedValue;
        TargetCamFOV = fixedValue
            ? (DefaultCamFOV - targetFov) * -1f
            : targetFov;
        CamFovLerpTime = lerpTime;
    }

    public void ForceCameraFov(float targetFov, bool fixedValue = false)
    {
        SetCameraFov(targetFov, CamFovLerpTime, fixedValue);
        CurrentCamFOV = Mathf.Clamp(TargetCamFOV + DefaultCamFOV, 1f, 110f);
    }

    public void ChangeDefFov(float fov)
    {
        var perfactFov = DefaultCamFOV + TargetCamFOV;

        DefaultCamFOV = fov;
        if (isFixedFovValue)
        {
            SetCameraFov(perfactFov, CamFovLerpTime, true);
        }
        CurrentCamFOV = DefaultCamFOV;
    }

    public void SetClampHori(float yEulerAngle, float lockedMaxAngle = 45f)
    {
        _setClampHori = yEulerAngle;
        if (_setClampHori > 360)
        {
            var scalar = _setClampHori / 360;
            _setClampHori -= Mathf.CeilToInt(scalar) * 360;
        }
        //수평각도 잠금 시 기준각도 월드방향 계산
        if (_setClampHori > 180 && _clampHori < _setClampHori - 180)
        {
            _clampHori += 360;
        }
        else
        {
            var setf = _clampHori - _setClampHori;
            _clampHori = setf > 180 ? _clampHori - 360 : _clampHori;
        }
        _clampHorizontalAngle = true;
        _maxHorizontalAngle = lockedMaxAngle;
    }

    public void SetClampHori(Vector3 Dir, float lockedMaxAngle = 45f)
    {
        var removeYDir = new Vector3(Dir.x, 0f, Dir.z);
        removeYDir = removeYDir.sqrMagnitude > 0f ? removeYDir.normalized : CamHolder.transform.forward;
        SetClampHori(Quaternion.LookRotation(removeYDir).eulerAngles.y, lockedMaxAngle);
    }

    public void SetClampVert(float up = -60f, float down = 30f)
    {
        _maxSoftClampVertUp = up;
        _maxSoftClampVertDown = down;
        _softClampVerticalAngle = true;
    }

    public void ResetClampHori()
    {
        _clampHorizontalAngle = false;
    }

    public void ResetClampVert()
    {
        _softClampVerticalAngle = false;
    }

    public void SetClampTime(float horiT = 6f, float vertT = 6f)
    {
        _lerpClampHoriTime = horiT;
        _lerpClampVertTime = vertT;
    }

    private void Awake()
    {
        InitializeController();
    }

    private void Update()
    {
        if (_inputInfo.WeaponSkill)
        {
            SetCameraFov(-48f, 12f);
        }
        else if (_inputInfo.SlimeThrowing)
        {
            SetCameraFov(-8f, 5f);
        }
        else
        {
            SetCameraFov(0f, 8f);
        }
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            ChangeFov();
        }
        /*if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 a = new Vector3(-8, Random.Range(-2.0f, 2.0f), 0);
            AddViewPunch(a);
        }*/

        //fov 처리
        var finalFov = Mathf.Clamp(TargetCamFOV + DefaultCamFOV, 1f, 110f);
        CurrentCamFOV = Mathf.Lerp(CurrentCamFOV, finalFov, CamFovLerpTime * Time.deltaTime);
        PlayerCam.fieldOfView = CurrentCamFOV;
        onChangedFov?.Invoke(CurrentCamFOV);
    }

    void OnUpdate(float deltaTime)
    {
        var fovFactor = CurrentCamFOV / DefaultCamFOV;
        var lookY = _inputInfo.Look.y * fovFactor;
        var lookX = _inputInfo.Look.x * fovFactor;

        //수평잠금각도 선형보간
        if (_clampHorizontalAngle)
        {
            _lerpHori = Mathf.Lerp(_lerpHori, _maxHorizontalAngle, _lerpClampHoriTime * deltaTime);
        }
        else
        {
            _lerpHori = 180f;
        }

        _clampVert -= lookY;
        _clampVert = Mathf.Clamp(_clampVert, -_maxVerticalAngle, _maxVerticalAngle);
        if (_softClampVerticalAngle)
        {
            if (_clampVert > _maxSoftClampVertDown)
            {
                _clampVert = Mathf.Lerp(_clampVert, _maxSoftClampVertDown, _lerpClampVertTime * deltaTime);
            }
            else if (_clampVert < _maxSoftClampVertUp)
            {
                _clampVert = Mathf.Lerp(_clampVert, _maxSoftClampVertUp, _lerpClampVertTime * deltaTime);
            }
        }

        if (_clampHorizontalAngle)
        {
            _clampHori += lookX;
            _clampHori = Mathf.Clamp(_clampHori, _setClampHori - _lerpHori, _setClampHori + _lerpHori);
        }
        else
        {
            _clampHori = transform.eulerAngles.y + lookX;
        }

        //내가 왜이랬을까.
        changedHori = ((_lastHoriValue - _clampHori >= 0) ? _lastHoriValue - _clampHori : -(_lastHoriValue - _clampHori)) > 0f
            ? lookX
            : 0f;
        changedVert = ((_lastVertValue - _clampVert >= 0) ? _lastVertValue - _clampVert : -(_lastVertValue - _clampVert)) > 0f
            ? lookY
            : 0f;

        _lastHoriValue = _clampHori;
        _lastVertValue = _clampVert;

        // 최종 각도 대입
        AltCamHolder.localRotation = Quaternion.Euler(_clampVert, 0f, 0f);
        CamHolder.localRotation = Quaternion.Euler(0f, _clampHori, 0f);
    }

    private void LateUpdate()
    {
        //시점계산
        if(GameManager.instance.state == GameManager.GameState.Start)
        {
            OnUpdate(Time.deltaTime);
        }
        
        //뷰펀치계산
        DecayPunchAngle();
        _currentLocalRot = Vector3.Lerp(_currentLocalRot, _vecPunchAngle, 25f * Time.deltaTime);
        AltCamHolder.localEulerAngles += _currentLocalRot;

        //UpdateCameraPositionToFollowTransform();
        //CurrentCameraPosition = Vector3.Lerp(CurrentCameraPosition, _followTransform.position, 1f - Mathf.Exp(-10000f * Time.deltaTime));
        transform.position = _followTransform.position;
    }

    public Vector3 CurrentCameraPosition { get; private set; }

    public void UpdateCameraPositionToFollowTransform()
    {
        CurrentCameraPosition = Vector3.Lerp(CurrentCameraPosition, _followTransform.position, 1f - Mathf.Exp(-10000f * Time.deltaTime));
        transform.position = _followTransform.position;
    }


    private void DecayPunchAngle()
    {
        if (_vecPunchAngle.sqrMagnitude > 0.001f || _vecPunchAngleVel.sqrMagnitude > 0.001f)
        {
            _vecPunchAngle += _vecPunchAngleVel * Time.deltaTime;
            float damping = 1 - (PUNCH_DAMPING * Time.deltaTime);

            if (damping < 0)
            {
                damping = 0;
            }
            _vecPunchAngleVel *= damping;

            float springForceMag = PUNCH_SPRING_CONSTANT * Time.deltaTime;
            springForceMag = Mathf.Clamp(springForceMag, 0f, 2f);
            _vecPunchAngleVel -= _vecPunchAngle * springForceMag;
        }
        else
        {
            _vecPunchAngle = Vector3.zero;
            _vecPunchAngleVel = Vector3.zero;
        }
    }

    public void SetPunchAngle(Vector3 angle) => _vecPunchAngle = angle;

    public void AddViewPunch(Vector3 angleVel) => _vecPunchAngleVel += angleVel * 20f;


    int FovT = 1;


    void ChangeFov()
    {
        FovT++;
        FovT %= 3;
        var TG = 0f;
        switch (FovT)
        {
            case 0:
                TG = 60f;
                break;
            case 1:
                TG = 75f;
                break;
            case 2:
                TG = 90f;
                break;
        }

        ChangeDefFov(TG);
    }
}