using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rattling_container : MonoBehaviour

{
     public Animation rattlingHere;
     public AudioSource rattlingSFXHere;
  
    void OnTriggerStay (Collider ghostHere)
    {
        if (ghostHere.tag=="Ghost")

        {
            if(Input.GetKeyDown(KeyCode.E))
            
            {
                rattlingHere.Play();
                {

                    Debug.Log("rattled");
                    {
                        rattlingSFXHere.Play();

                    }
                }

                

            }

        }
    }

}
