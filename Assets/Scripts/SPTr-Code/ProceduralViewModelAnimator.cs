using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralViewModelAnimator : MonoBehaviour
{
    [SerializeField] private LandEffectSimluator _landEffecter;
    [SerializeField] private Camera _fpsCamera;
    [SerializeField] private float _inputLerpSpeed = 25f;
    [SerializeField] private PlayerInputInfo _inputInfo;
    [SerializeField] private MoveController _moveCon;
    [SerializeField] private LookController _lookCon;

    private Vector3 _savedLocalPos;
    private Vector3 _localSwayPos;
    private Quaternion _localSwayRot;

    [Header("시점이동 흔들림")]
    private float _swayX;
    private float _swayY;
    private float _targetSwayX;
    private float _targetSwayY;
    public float maxSwayAmount = 5f;
    public float handlingAmount = -2f;
    public float handlingSharpness = 10f;

    [Header("이동속력 기울기")]
    private float _targetMoveTiltX;
    private float _targetMoveTiltY;
    private float _currentMoveTiltX;
    private float _currentMoveTiltY;
    public float moveTiltAmount = 4f;
    public float moveTiltSharpness = 6f;

    [Header("랜드 이펙팅 및 시야각 스케일링")]
    public float landEffectScale = 0.1f;
    public float scaler = 0.1f;
    public int FovT = 0;

    private float _currentFovScale;

    //야간산책용 옵션
    //[Header("애니메이션 디폴트")]
    //public bool cutAnimationFrame = false;
    //public int frameRate = 24;
    //private float _frameRateTime = 0f;

    public void Initialize()
    {
        _savedLocalPos = transform.localPosition;
        _localSwayPos = _savedLocalPos;
        _currentFovScale = _fpsCamera.fieldOfView / 75f;
        FovT = 1;
    }

    private void Awake()
    {
        _moveCon.onPlayerLanded += this.OnLanded;
    }

    void Start()
    {
        Initialize();
    }

    //private void OnValidate()
    //{
    //    if (cutAnimationFrame)
    //    {
    //        _frameRateTime = 1f / frameRate;
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale <= 0.0f) return;

        //인풋 읽기
        SwayInput(new Vector2(_lookCon.changedHori, _lookCon.changedVert));
        TiltHandDir();

        //알고리즘 수행
        var lookAmount = new Vector2(_swayX, _swayY) * (handlingAmount * 0.005f * _currentFovScale);
        var newPos = _savedLocalPos + new Vector3(lookAmount.x, lookAmount.y,0f) + (Vector3.right * _currentMoveTiltX + Vector3.up * _currentMoveTiltY) * (0.01f * _currentFovScale);

        _localSwayPos = Vector3.Lerp(_localSwayPos, newPos, handlingSharpness * Time.smoothDeltaTime);

        var clampedSwayPos = _localSwayPos + Vector3.up * _landEffecter.Yvalue;
        for (int i = 0; i < 3; i++)
        {
            if (Mathf.Abs(clampedSwayPos[i]) < 0.001f * scaler)
            {
                clampedSwayPos[i] = 0.0f;
            }
        }

        float tiltZ = -_swayX + _currentMoveTiltX + _currentMoveTiltY * -0.3f;
        if (Mathf.Abs(tiltZ) < 0.001f) tiltZ = 0.0f;
        float tilty = -_swayX * 3f;

        _localSwayRot = Quaternion.Slerp(_localSwayRot, Quaternion.Euler(0f, tilty, tiltZ), handlingSharpness * Time.smoothDeltaTime);

        //애니메이션 프레임 컷팅
        //_frameRateTime += Time.deltaTime;
        //if (cutAnimationFrame && _frameRateTime < 1f / frameRate)
        //{
        //    return;
        //}

        //최종 적용
        transform.SetLocalPositionAndRotation(clampedSwayPos, _localSwayRot);
        //_frameRateTime = 0f;
    }

    public void SwayInput(Vector2 Input)
    {
        _targetSwayX = Input.x * (1f / Time.deltaTime) * 0.005f * _currentFovScale;
        _targetSwayY = Input.y * (1f / Time.deltaTime) * 0.005f * _currentFovScale;

        _targetSwayX = Mathf.Clamp(_targetSwayX, -maxSwayAmount, maxSwayAmount);
        _targetSwayY = Mathf.Clamp(_targetSwayY, -maxSwayAmount, maxSwayAmount);

        _swayX = Mathf.Lerp(_swayX, _targetSwayX, _inputLerpSpeed * Time.deltaTime);
        _swayY = Mathf.Lerp(_swayY, _targetSwayY, _inputLerpSpeed * Time.deltaTime);
    }

    public void TiltHandDir()
    {
        var planeVel = new Vector3(_moveCon.cc.velocity.x, 0f, _moveCon.cc.velocity.z);
        planeVel = Vector3.ClampMagnitude(planeVel / _moveCon.moveSpeed, 1.0f);
        var rightDir = _lookCon.CamHolder.rotation * Vector3.right;
        var frontDir = _lookCon.CamHolder.rotation * Vector3.forward;

        var input = new Vector2(Vector3.Dot(rightDir, planeVel),
            Vector3.Dot(frontDir,planeVel));

        _targetMoveTiltX = -input.x * moveTiltAmount;
        _targetMoveTiltY = -input.y * moveTiltAmount;
        _currentMoveTiltX = Mathf.Lerp(_currentMoveTiltX, _targetMoveTiltX, moveTiltSharpness * Time.deltaTime);
        _currentMoveTiltY = Mathf.Lerp(_currentMoveTiltY, _targetMoveTiltY, moveTiltSharpness * Time.deltaTime);
    }

    public void OnLanded(float landForce)
    {
        _landEffecter.AddForce(landForce * landEffectScale);
    }

}
