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
        //    Debug.Log("Photon 서버에 연결되었습니다.");
        //}
    }

    public void Conncet() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnected()
    {
        base.OnConnected();
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
        Debug.Log("Photon 서버에 연결되었습니다.");

        logText.text += "PhotonServer Connected\n"; //updateTxt(out tempTxt,); 
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
        Debug.Log("마스터 서버에 연결되었습니다.");
        logText.text += "MasterServer Connected\n";

        //자동 로비 입장
        JoinLobby();
    }

    public void JoinLobby() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
        Debug.Log("로비에 입장했습니다.");
        logText.text += "Lobby Enter Success\n";

        // LobbyScene 으로 이동
        SceneManager.LoadScene("LobbyScene");
    }
}
