using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitableHPUI : MonoBehaviour
{
    Slider _slider;
    Image _fillImage;
    Color _dopColor;
    float _dangerTimer;
    public HitableObj hit;
    public bool dangerEffect;
    [Range(0.0f,1.0f)]
    public float dangerPercent = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        _slider = GetComponent<Slider>();
        _fillImage = _slider.fillRect.GetComponent<Image>();
        _dopColor = _fillImage.color;
    }

    // Update is called once per frame
    void Update()
    {
        _slider.value = hit.Hp / hit.maxHp;
        if (dangerEffect && _slider.value <= dangerPercent)
        {
            _dangerTimer += Time.deltaTime;
            _fillImage.color = new Color(1,0.3f, 0.1f, Mathf.Lerp(0.5f,1f, 1 - Mathf.Sin(6 * _dangerTimer)));
        }
        else
        {
            _dangerTimer = 0f;
            _fillImage.color = _dopColor;
        }
    }
}
