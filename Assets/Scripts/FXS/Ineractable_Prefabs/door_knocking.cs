using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door_knocking : MonoBehaviour
{
     public AudioSource doorHere;
  
    void OnTriggerStay (Collider ghostHere)
    {
        if (ghostHere.tag=="Ghost")

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
