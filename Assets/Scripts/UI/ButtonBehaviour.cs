using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ButtonBehaviour : MonoBehaviourPunCallbacks
{
    public void ExitGameButton(string mapName){
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(mapName);
        //NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All);
    }
}
