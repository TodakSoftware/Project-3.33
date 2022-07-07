using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI canvasTimerUI; // just for ghost
    public GameObject captureTextUI; // just for ghost

    void Start(){
        if(canvasTimerUI != null){
            canvasTimerUI.gameObject.SetActive(true);
        }
    }

    void Update(){
        if(canvasTimerUI != null){
            canvasTimerUI.text = GameManager.instance.timerRef;
        }
    } // end Update()
}
