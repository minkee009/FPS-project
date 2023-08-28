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
            authTxt.text = "���鹮�ڷ� ID�� ������ �� �����ϴ�.";
            return;
        }

        if (PlayerPrefs.HasKey(id.text))
        {
            authTxt.text = "�̹� �����ϴ� ID �Դϴ�.";
            return;
        }

        PlayerPrefs.SetString(id.text,pw.text);
    }

    public void LoginUser()
    {
        if(id.text == string.Empty)
        {
            authTxt.text = "ID�� �������� �ʽ��ϴ�.";
            return;
        }

        if (!PlayerPrefs.HasKey(id.text))
        {
            authTxt.text = "ID�� �������� �ʽ��ϴ�.";
            return;
        }
        if (pw.text == PlayerPrefs.GetString(id.text))
        {
            SceneManager.LoadScene(1);
        }
    }
}
