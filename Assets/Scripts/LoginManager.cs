using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public static LoginManager instance;
    public TMP_InputField id;
    public TMP_InputField pw;
    public TMP_Text authTxt;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if(instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Start()
    {
        authTxt.text = string.Empty;
    }

    public void RegistUser()
    {
        if (id.text == string.Empty)
        {
            authTxt.text = "공백문자로 ID를 생성할 수 없습니다.";
            return;
        }

        if (PlayerPrefs.HasKey(id.text))
        {
            authTxt.text = "이미 존재하는 ID 입니다.";
            return;
        }

        PlayerPrefs.SetString(id.text,pw.text);
    }

    public void LoginUser()
    {
        if(id.text == string.Empty)
        {
            authTxt.text = "ID가 존재하지 않습니다.";
            return;
        }

        if (!PlayerPrefs.HasKey(id.text))
        {
            authTxt.text = "ID가 존재하지 않습니다.";
            return;
        }
        if (pw.text == PlayerPrefs.GetString(id.text))
        {
            SceneManager.LoadScene(1);
        }
    }
}
