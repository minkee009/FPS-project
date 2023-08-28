using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandEffectSimluator : MonoBehaviour
{
    public float Yvalue { get; private set; }  

    public enum MasspointCollision
    {
        None,
        Hard,
        Soft,
        reflect
    }

    [Tooltip("질점 충돌 :: 질점이 고정점에 닿았을 경우 속도를 어떻게 처리할지에 대한 모드값입니다.")]
    public MasspointCollision CollisionMode = MasspointCollision.Hard;

    public float anchoringPointYpos = 0f;
    public float springFactor = 24f;
    public float dampingFactor = 6f;
    public float maxLength = 2f;

    public bool doSimulateMotion = true;

    private float _velocity;
    private float _internalYPosition;

    // Start is called before the first frame update
    void Start()
    {
        InitialzePosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (doSimulateMotion)
        {
            //SpringForce 구하기
            var springForce = -springFactor * ((_internalYPosition - anchoringPointYpos) - maxLength);
            var dampingForce = -dampingFactor * _velocity;

            //SpringForce 적용
            _velocity += (springForce + dampingForce) * Time.deltaTime;
            _internalYPosition += _velocity * Time.deltaTime;

            //질점 충돌 계산 및 적용
            if (_internalYPosition - anchoringPointYpos <= 0 && CollisionMode != MasspointCollision.None)
            {
                switch (CollisionMode)
                {
                    case MasspointCollision.Hard:
                        {
                            _velocity -= _velocity;
                            break;
                        }
                    case MasspointCollision.Soft:
                        {
                            _velocity += anchoringPointYpos - _internalYPosition;
                            break;
                        }
                    case MasspointCollision.reflect:
                        {
                            _velocity += anchoringPointYpos - _internalYPosition;
                            _velocity *= -1f;
                            break;
                        }
                }
                _internalYPosition = anchoringPointYpos;
            }

            Yvalue = _internalYPosition;
        }
        else
        {
            _velocity = 0f;
            _internalYPosition = Mathf.Lerp(_internalYPosition, anchoringPointYpos + maxLength, 12f * Time.deltaTime);
        }       
    }


    public void InitialzePosition()
    {
        _internalYPosition = anchoringPointYpos + maxLength;
    }

    public void AddForce(float force)
    {
        _velocity -= force;
    }
}
