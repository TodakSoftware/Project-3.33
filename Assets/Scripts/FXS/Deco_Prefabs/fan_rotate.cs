using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fan_rotate : MonoBehaviour

{

    //public GameObject playerHere;
    //public float speedHere;
    public Animation fanHere;
    public float fanSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
      fanHere["Rotating_Fan"].speed=fanSpeed;
      Rotating();
    }

    // Update is called once per frame
    void Update()
    {
    
       
    }

    void OnTriggerEnter(Collider playerHere)
     
    {

        //if (playerHere.tag=="Player")
        {
            //Debug.Log("it worked");
            Rotating();
        }
        
     
      
    
    }

      void OnTriggerExit(Collider playerHere)
     
    {

        //if (playerHere.tag=="Player")
            {
                //Debug.Log("bye-bye");
                StopRotating();

            }
            

        
       
      
    
    }

      void Rotating()
     
    {

        //if (gameObject.tag=="Player")
        fanHere.Play("Rotating_Fan");
        //transform.RotateAround(kipasHere.transform.position, Vector3.up, speedHere * Time.deltaTime);
      
    
    }

void StopRotating()
{
    //fanHere.Stop();
}

}
