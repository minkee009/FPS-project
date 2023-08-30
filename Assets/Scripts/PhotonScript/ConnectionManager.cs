using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    //private void Start()
    //{
    //    Conncet();
    //}
    public TMP_Text logText;

    private void Update()
    {
        //if (PhotonNetwork.IsConnected)
        //{
        //    Debug.Log("Photon ������ ����Ǿ����ϴ�.");
        //}
    }

    public void Conncet() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnected()
    {
        base.OnConnected();
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
        Debug.Log("Photon ������ ����Ǿ����ϴ�.");

        logText.text += "PhotonServer Connected\n"; //updateTxt(out tempTxt,); 
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
        Debug.Log("������ ������ ����Ǿ����ϴ�.");
        logText.text += "MasterServer Connected\n";

        //�ڵ� �κ� ����
        JoinLobby();
    }

    public void JoinLobby() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
        Debug.Log("�κ� �����߽��ϴ�.");
        logText.text += "Lobby Enter Success\n";

        // LobbyScene ���� �̵�
        SceneManager.LoadScene("LobbyScene");
    }
}
