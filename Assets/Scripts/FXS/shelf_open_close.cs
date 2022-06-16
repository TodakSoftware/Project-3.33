using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shelf_open_close : MonoBehaviour



{

    public Animation doorShelfHere;
    public AudioSource audioShelfHere;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider ghostHere)
     
    {
    if (ghostHere.tag=="Ghost")
        
        {
            if(Input.GetKey(KeyCode.E))
            doorShelfHere.Play();

            if(Input.GetKey(KeyCode.E))
            audioShelfHere.Play();


        
        }

        }
       

            }

        
        
    



