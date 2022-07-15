using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tower_pulsating_light : MonoBehaviour
{

    private Animation towerLight;
    // Start is called before the first frame update
    void Start()
    {
        if(towerLight !=null)
        towerLight.Play();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
