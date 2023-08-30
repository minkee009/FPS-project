using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public TMP_Text infoText;

    public void ShowRoomInfo()
    {
        if (PhotonNetwork.InRoom)
        {
            //필요 내용 : 방 이름, 방 인원수, 최대 인원수, 플레이어 이름
            string roomName = PhotonNetwork.CurrentRoom.Name;
            int playerCnt = PhotonNetwork.CurrentRoom.PlayerCount;
            int maxPlayerCnt = PhotonNetwork.CurrentRoom.MaxPlayers;

            string playerNames = "< Player List >\n";
            for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                playerNames += PhotonNetwork.PlayerList[i].NickName + "\n";
            }

            infoText.text = string.Format("Room : {0} \n Player Number : {1} \n Max Player Number : {2} \n{3} ", roomName, playerCnt, maxPlayerCnt, playerNames);
        }
    }

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        SceneManager.LoadScene(1);
    }
}
