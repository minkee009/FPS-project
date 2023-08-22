using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitableHPUI : MonoBehaviour
{
    Slider _slider;
    public HitableObj hit;

    // Start is called before the first frame update
    void Start()
    {
        _slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        _slider.value = hit.Hp / hit.maxHp;
    }
}
