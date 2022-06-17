using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door_open_close : MonoBehaviour
{

    public Animation doorHere;
    public AudioSource doorsoundHere;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay (Collider ghostHere){


         if (ghostHere.tag=="Ghost")
        
        {
            if(Input.GetKey(KeyCode.E))
            doorHere.Play();
           

            if(Input.GetKey(KeyCode.E))
            doorsoundHere.Play();
            


        
        }

        }
       

            }
