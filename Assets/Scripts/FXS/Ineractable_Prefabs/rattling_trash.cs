using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rattling_trash : MonoBehaviour
{
     public Animation rattlingtrashHere;
     public AudioSource rattlingSFXHere;
     public bool delayOn;

      IEnumerator Start(){

        yield return new WaitForSeconds(3f);
        delayOn=true;
     }
  
    void OnTriggerStay (Collider ghostHere)
    {
        if (ghostHere.tag=="Ghost" && delayOn)

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
