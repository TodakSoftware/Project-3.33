using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flickering_lights : MonoBehaviour
{
    public Light lampSini;
    public float minTime;
    public float maxTime;
    public float Timer;
    public AudioSource soundSini;
    public AudioClip audioSini;

    public GameObject bulbSini;

    // Start is called before the first frame update
    void Start()
    {
        Timer=Random.Range(minTime,maxTime);
      
        
    }

     void Update()
    {
        flickeringLights();
    }

    // Update is called once per frame
    void flickeringLights()
    {
        if(Timer>0)
        Timer-=Time.deltaTime;

        

        if(Timer<=0)



        {
            lampSini.enabled=!lampSini.enabled;
            bulbSini.SetActive(!bulbSini.activeSelf);
            Timer=Random.Range(minTime,maxTime);
            soundSini.PlayOneShot(audioSini);
            
            
        }
}

}
