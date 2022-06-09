using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Modal_ReconnectGame : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI reconnectText;
    public Button reconnectButton, leaveButton;

    void Start(){
        reconnectButton.onClick.AddListener(delegate{ReconnectButtonPressed();});
        leaveButton.onClick.AddListener(delegate{LeaveButtonPressed();});
    }

    void ReconnectButtonPressed(){
        reconnectText.text = "Reconnecting...";
        NetworkManager.instance.ReconnectToGame();
    }

    void LeaveButtonPressed(){
        reconnectText.text = "Leaving room...";
        StartCoroutine(UIManager.instance.CloseReconnectGame(.3f)); // Close if not connected
        NetworkManager.instance.CancelFindGameOrLeaveRoom();
        
    }
}
