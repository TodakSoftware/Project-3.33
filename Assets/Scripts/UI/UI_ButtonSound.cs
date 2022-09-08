using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AK.Wwise.Event clickEventName = new AK.Wwise.Event();
    public AK.Wwise.Event hoverEventName = new AK.Wwise.Event();

    public void OnPointerClick(PointerEventData eventData)
    {
        if(GetComponent<Button>().interactable){
            if(clickEventName.Name != "")
                AkSoundEngine.PostEvent(clickEventName.Name, gameObject);
            else
                print("clickEventName variable is empty" + this.gameObject);
        }// end if interactable
    }// end OnPointerEnter

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(GetComponent<Button>().interactable){
            if(hoverEventName.Name != "")
                AkSoundEngine.PostEvent(hoverEventName.Name, gameObject);
            else
                print("hoverEventName variable is empty on " + this.gameObject);
        } // end if interactable
    } // end OnPointerEnter
}
