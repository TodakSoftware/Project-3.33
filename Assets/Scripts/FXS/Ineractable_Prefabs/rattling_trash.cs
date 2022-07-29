using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rattling_trash : MonoBehaviour
{
     public Animation rattlingtrashHere;
     public AudioSource rattlingSFXHere;
  
    void OnTriggerStay (Collider ghostHere)
    {
        if (ghostHere.tag=="Ghost")

        {
            if(Input.GetKeyDown(KeyCode.E))
            
            {
                rattlingtrashHere.Play();
                {

                    Debug.Log("rattled");
                }
                {

                    rattlingSFXHere.Play();

                }

            }

        }
    }

}
