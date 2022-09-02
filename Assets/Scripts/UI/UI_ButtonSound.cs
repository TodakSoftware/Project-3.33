using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public string hoverClipName = "UI_Btn_Hover";
    public string clickClipName = "UI_Btn_Click";

    public void OnPointerClick(PointerEventData eventData)
    {
        if(AudioManager.instance != null){
            AudioManager.instance.PlaySound(clickClipName);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(AudioManager.instance != null){
            AudioManager.instance.PlaySound(hoverClipName);
        }
    }
}
