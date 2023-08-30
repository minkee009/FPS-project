using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ���� : ����� ���� 
public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomNameInput;
    public int maxPlayerNum = 5;
    public TMP_Text logText;

    public int gameSceneNum = 2;

    //����� ���� �κ�
    public void CreateRoom()
    {
        //���� �ִ� ������ InputField��, ����� ���� InputField�� ��������
        if(roomNameInput.text != "")
        {
            PhotonNetwork.JoinOrCreateRoom(roomNameInput.text, new Photon.Realtime.RoomOptions { MaxPlayers = maxPlayerNum }, null);
            Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
        Debug.Log("�濡 �����߽��ϴ�.");

        logText.text += "Enter Success\n";

        SceneManager.LoadScene(gameSceneNum);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
        Debug.Log("���� �����߽��ϴ�.");

        logText.text += "Create Room Success\n";
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
        Debug.Log("�� ������ �����߽��ϴ�.");

        logText.text = "Create Room Failed\n";
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
        Debug.Log("�Ա��� ���߽��ϴ�. ���� : ��?��");

        logText.text += "Failed to join\n";

        
    }
}
