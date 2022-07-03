using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_VictoryResult : MonoBehaviour
{
    public TextMeshProUGUI teamText;

    public void HumanWin(){
        teamText.text = "Human Victory!";
    }

    public void GhostWin(){
        teamText.text = "Ghost Victory!";
    }
}
