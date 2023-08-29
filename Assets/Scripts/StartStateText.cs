using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartStateText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.stateText = GetComponent<TMP_Text>();
    }
}
