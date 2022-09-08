using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundRTPC : MonoBehaviour {
    public AK.Wwise.RTPC humanFearLevelRTPC;
    public Human humanInstance; // human scripts references
    
    // Update is called once per frame.

    public void PlayFearLevelSound(){
        humanFearLevelRTPC.SetValue(gameObject, (float)humanInstance.fearLevel);
    }
}
