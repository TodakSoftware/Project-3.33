using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFearLevelRTPC : MonoBehaviour {
    public AK.Wwise.RTPC HumanFearLevelRTPC;
    public Human humanInstance;
    float convertedValue;

    // Use this for initialization.
    void Start () {
        humanInstance = GetComponent<Human>();
    }
    
    // Update is called once per frame.
    void Update () {
        convertedValue = (float)humanInstance.fearLevel;
        HumanFearLevelRTPC.SetValue(gameObject, convertedValue);
    }
}
