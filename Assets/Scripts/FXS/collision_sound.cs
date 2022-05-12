using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collision_sound : MonoBehaviour
{

public AudioSource soundHere;
    // Start is called before the first frame update
    void Start()
    {
         soundHere = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter (Collision collision){

        //if (collision.gameObject.tag=="Floor")
        if (collision.gameObject.tag=="Player")
           
        
        {
            

            if(gameObject != null) soundHere.Play();

            
        }
    }
}
