using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door_knocking : MonoBehaviour
{

     public AudioSource doorHere;
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
                doorHere.Play();
                {

                    Debug.Log("knock!");
                }

            }

        }
    }

}
