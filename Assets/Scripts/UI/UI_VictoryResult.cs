using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class UI_VictoryResult : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI teamText;
    public TextMeshProUGUI redirectText;
    float redirectDuration = 5f;

    public void HumanWin(){
        teamText.text = "Human Victory!";
        if(gameObject.activeSelf)
        StartCoroutine(RedirectAfterEndGame());
    }

    public void GhostWin(){
        teamText.text = "Ghost Victory!";
        if(gameObject.activeSelf)
        StartCoroutine(RedirectAfterEndGame());
    }

    IEnumerator RedirectAfterEndGame(){

        while(redirectDuration > 0){
            redirectText.text = "Redirect to main menu in "+redirectDuration;
            yield return new WaitForSeconds(1f);
            redirectDuration -= 1;
        }

        if(redirectDuration <= 0){
            //PhotonNetwork.LeaveRoom();
            if(photonView.IsMine){
                PhotonNetwork.LeaveRoom();
                //NetworkManager.instance.CancelFindGameOrLeaveRoom();
                PhotonNetwork.LoadLevel("MainMenu");
            }
        }
    }
}
