using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputInfo : MonoBehaviour
{
    private PlayerActions actions;

    //출력 변수
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    public bool Jump { get; private set; }
    public bool JumpHold { get; private set; }
    public bool SprintPressed { get; private set; }
    public bool SprintHold { get; private set; }
    public bool SprintReleased { get; private set; }
    public bool Crouch { get; private set; }
    public bool CrouchHold { get; private set; }
    public bool Attack { get; private set; }
    public bool SlimeThrowing { get; private set; }
    public bool WeaponSkill { get; private set; }
    public bool UseItem { get; private set; }



    //설정값 변수
    public float LookSensitivity = 3.0f;
    public float LookGamepadHorizontalScale = 1.0f;
    public float LookGamepadVerticalScale = 1.0f;
    public bool LookGamepadAcceleration = true;
    public float LookGamepadAccelMaxSpeed = 1.3f;
    public AnimationCurve LookAccelMotion;
    public AnimationCurve StickCorrectionCurve;
    [Range(0f, 45f)]
    public float StickCorrectionAngle = 45f;


    public bool LookSmoothing = true;
    public bool StickCorrection = true;
    public bool LookGamepadInterpolation = false;

    public bool HoldSprint = false;
    public bool HoldCrouch = false;

    //내부 변수
    private float ki_look_gamepad_momentum = 0f;
    private float ki_look_gamepad_accel = 1f;
    private Vector2 ki_look_gamepad_currentLerp = Vector2.zero;

    //내부 트리거
    private bool tg_attack = false;

    //상수
    private const float TIMER_KEY_GPADLOOK = 0.3f;

    //인풋스무딩
    private static int _maxIndexCount = 3;
    private int _currentIndex = 0;
    private float[] _horiArray = new float[_maxIndexCount];
    private float[] _vertArray = new float[_maxIndexCount];

    #region 인풋스무딩 메서드

    public void ResetLookInputSmooth()
    {
        _currentIndex = 0;
        for (int i = 0; i < _maxIndexCount; i++)
        {
            _horiArray[i] = 0f;
            _vertArray[i] = 0f;
        }
    }

    private void IncreaseIndexCount()
    {
        _currentIndex++;
        _currentIndex %= _maxIndexCount;
    }

    private float GetAverageHori(float hori)
    {
        _horiArray[_currentIndex] = hori;
        float result = 0f;

        for (int i = 0; i < _horiArray.Length; i++)
        {
            result += _horiArray[i];
        }
        return result / _horiArray.Length;
    }

    private float GetAverageVert(float vert)
    {
        _vertArray[_currentIndex] = vert;
        float result = 0f;

        for (int i = 0; i < _vertArray.Length; i++)
        {
            result += _vertArray[i];
        }
        return result / _vertArray.Length;
    }

    #endregion

    private void Awake()
    {
        actions = new PlayerActions();
        Application.targetFrameRate = -1;
    }

    private void Update()
    {
        UpdateInputInfo(Time.deltaTime);

        //디버깅용 삭제필요
        if (Input.GetKeyDown(KeyCode.F5))
        {
            _frameRateLimitMode++;
            _frameRateLimitMode %= 3;
            SwitchFrameRateLimitMode();
        }
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            HoldCrouch = !HoldCrouch;
        }
        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            HoldSprint = !HoldSprint;
        }
    }

    public void OnEnable()
    {
        actions.Enable();
    }

    public void OnDisable()
    {
        actions.Disable();
    }

    #region 인풋업데이트
    void UpdateInputInfo(float deltaTime)
    {
        #region 합산 입력

        // 초기화
        Vector2 initMove = Vector2.zero;
        Vector2 initLook = Vector2.zero;

        //--- 키보드 입력처리
        initMove += actions.InGames.Move_keyboard.ReadValue<Vector2>();

        //--- 마우스 입력처리
        initLook += actions.InGames.Look_mouse.ReadValue<Vector2>() * 0.02f;

        //--- 게임패드 입력처리
        Vector2 padMove = actions.InGames.Move_gamepad.ReadValue<Vector2>();
        if (StickCorrection)
        {
            /*
            for (int j = 0; j < 2; j++)
            {
                int signF = padMove[j] > 0 ? 1 : -1;
                padMove[j] = StickCorrectionCurve.Evaluate(MathF.Abs(padMove[j])) * signF;
            }
            */

            var correctAngle = Vector2.Dot(Vector2.right, padMove.normalized) > 0
                ? Vector2.Angle(Vector2.up, padMove)
                : 360 - Vector2.Angle(Vector2.up, padMove);

            var scalar = (Mathf.CeilToInt(correctAngle / 90) - 1) * 90;

            var sca = StickCorrectionAngle * 0.5f;

            var demp = Mathf.Clamp(correctAngle - scalar, sca, 90 - sca);

            demp = (demp - sca) / (90 - StickCorrectionAngle) * 90f + scalar;

            padMove = new Vector2(Mathf.Sin(demp * Mathf.Deg2Rad), Mathf.Cos(demp * Mathf.Deg2Rad)) * padMove.magnitude;

        }
        initMove += padMove;

        Vector2 gamepadLook = actions.InGames.Look_gamepad.ReadValue<Vector2>();
        if (StickCorrection)
        {

            var correctAngle = Vector2.Dot(Vector2.right, gamepadLook.normalized) > 0
                ? Vector2.Angle(Vector2.up, gamepadLook)
                : 360 - Vector2.Angle(Vector2.up, gamepadLook);

            var scalar = (Mathf.CeilToInt(correctAngle / 90) - 1) * 90;

            var sca = StickCorrectionAngle * 0.5f;

            var demp = Mathf.Clamp(correctAngle - scalar, sca, 90 - sca);

            demp = (demp - sca) / (90 - StickCorrectionAngle) * 90f + scalar;

            gamepadLook = new Vector2(Mathf.Sin(demp * Mathf.Deg2Rad), Mathf.Cos(demp * Mathf.Deg2Rad)) * gamepadLook.magnitude;


            /*
            for (int j = 0; j < 2; j++)
            {
                int signF = gamepadLook[j] > 0 ? 1 : -1;
                gamepadLook[j] = StickCorrectionCurve.Evaluate(MathF.Abs(gamepadLook[j])) * signF;
            }
            gamepadLook = Vector2.ClampMagnitude(gamepadLook, 1f);
            */

            //y = (x - 22.5) * 2f

            //Debug.Log(demp + scalar * 90);
        }
        float i = gamepadLook.x < 0 ? -1f : 1f;

        gamepadLook[0] *= LookGamepadHorizontalScale;
        gamepadLook[1] *= LookGamepadVerticalScale;

        if (LookGamepadAcceleration
            && Mathf.Abs(gamepadLook.x) > 0.58f)
        {
            ki_look_gamepad_momentum += ki_look_gamepad_accel * deltaTime;
            ki_look_gamepad_momentum = Mathf.Min(1f, ki_look_gamepad_momentum);

            gamepadLook[0] += LookAccelMotion.Evaluate(ki_look_gamepad_momentum) * i * LookGamepadAccelMaxSpeed;
        }
        else
        {
            ki_look_gamepad_momentum = 0f;
        }

        gamepadLook *= 40f * deltaTime;

        if (LookGamepadInterpolation)
        {
            ki_look_gamepad_currentLerp = Vector2.Lerp(ki_look_gamepad_currentLerp, gamepadLook, 32f * deltaTime);
            gamepadLook = ki_look_gamepad_currentLerp;
        }

        initLook += gamepadLook;


        // 보정
        initMove = Vector2.ClampMagnitude(initMove, 1f);
        initLook *= LookSensitivity;
        if (LookSmoothing)
        {
            initLook.x = GetAverageHori(initLook.x);
            initLook.y = GetAverageVert(initLook.y);
            IncreaseIndexCount();
        }


        // 적용
        Move = initMove;
        Look = initLook;

        #endregion

        #region 개별 입력

        // 초기화

        // 보정

        // 적용
        Jump = actions.InGames.Jump.WasPressedThisFrame();
        JumpHold = actions.InGames.Jump.IsPressed();
        Crouch = actions.InGames.Crouch.WasPressedThisFrame();
        CrouchHold = actions.InGames.Crouch.IsPressed();
        SprintPressed = actions.InGames.Sprint.WasPressedThisFrame();
        SprintHold = actions.InGames.Sprint.IsPressed();
        SprintReleased = actions.InGames.Sprint.WasReleasedThisFrame();
        SlimeThrowing = actions.InGames.SlimeThrowing.IsPressed();
        UseItem = actions.InGames.UseItem.WasPressedThisFrame();

        #endregion

        #region 트리거 입력

        // 초기화


        // 보정

        // 적용
        Attack = false;
        if (!tg_attack && actions.InGames.Attack.ReadValue<float>() > 0.65f)
        {
            tg_attack = true;
            Attack = true;
        }
        
        if(tg_attack && actions.InGames.Attack.ReadValue<float>() < 0.1f)
        {
            tg_attack = false;
        }

        var weaponSkillValue = actions.InGames.WeaponSkill.ReadValue<float>();
        if (!WeaponSkill && weaponSkillValue > 0.65f)
        {
            WeaponSkill = true;
        }
        if (WeaponSkill && weaponSkillValue < 0.4f)
        {
            WeaponSkill = false;
        }

        #endregion
    }

    #endregion

    private int _frameRateLimitMode = 0;

    void SwitchFrameRateLimitMode()
    {
        switch (_frameRateLimitMode)
        {
            //제한 없음
            case 0:
                Application.targetFrameRate = 0;
                break;

            //30 fps
            case 1:
                Application.targetFrameRate = 30;
                break;

            //60 fps
            case 2:
                Application.targetFrameRate = 60;
                break;
        }
    }
}