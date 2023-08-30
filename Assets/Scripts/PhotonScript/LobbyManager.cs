using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 목적 : 만든다 방을 
public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomNameInput;
    public int maxPlayerNum = 5;
    public TMP_Text logText;

    public int gameSceneNum = 2;

    //만든다 방을 로비에
    public void CreateRoom()
    {
        //만약 있다 내용이 InputField에, 만든다 방을 InputField의 내용으로
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
        Debug.Log("방에 입장했습니다.");

        logText.text += "Enter Success\n";

        SceneManager.LoadScene(gameSceneNum);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
        Debug.Log("방을 생성했습니다.");

        logText.text += "Create Room Success\n";
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
        Debug.Log("방 생성에 실패했습니다.");

        logText.text = "Create Room Failed\n";
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
        Debug.Log("입구컷 당했습니다. 사유 : 몰?루");

        logText.text += "Failed to join\n";

        
    }
}
