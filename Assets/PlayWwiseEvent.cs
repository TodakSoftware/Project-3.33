using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayWwiseEvent : MonoBehaviour
{
    public AK.Wwise.Event sound = new AK.Wwise.Event();
    public bool playOnEnable;

    public void OnEnable(){
        if(playOnEnable){
            if(sound.Name != "")
                AkSoundEngine.PostEvent(sound.Name, gameObject);
        }
    }

    public void Start(){
        if(!playOnEnable){
            if(sound.Name != "")
                AkSoundEngine.PostEvent(sound.Name, gameObject);
        }
    }
}
