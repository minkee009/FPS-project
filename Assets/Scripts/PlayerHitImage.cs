using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHitImage : MonoBehaviour
{
    Image image;

    public HitableObj hit;


    bool _isDeath;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        hit.OnHit += HitEffect;
        hit.OnDie += ChangeDeath;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isDeath)
        {
            image.color = Color.Lerp(image.color, new Color(image.color.r, image.color.g, image.color.b, 0f), 6f * Time.deltaTime);
        }
        else
        {
            image.color = Color.Lerp(image.color, new Color(image.color.r, image.color.g, image.color.b, 1f), 3f * Time.deltaTime);
        }        
    }

    public void HitEffect()
    {
        image.color += new Color(0, 0, 0, 0.34f);
    }

    public void ChangeDeath()
    {
        _isDeath = true;
    }
}
