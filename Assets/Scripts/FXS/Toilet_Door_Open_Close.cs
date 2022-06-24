using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Toilet_Door_Open_Close : MonoBehaviour

{

    public Animation doorHere;
    public AudioSource soundhere;

    public bool doorStatus;

    
   

    // Update is called once per frame
    void OnTriggerStay(Collider ghostHere)
    {
    

         if (ghostHere.CompareTag("Ghost"))
        
        {
            if(Input.GetKey(KeyCode.E))

              if(!doorStatus){

                OpenDoor();
                
            } else {

                CloseDoor();
       

            }
            
             }

        } // end OnTriggerStay

        void OpenDoor(){
            doorStatus=true;
        print("open works!");
        transform.DORotateQuaternion(new Quaternion(-0.5f,0.5f,0.5f,0.5f),3f);
        }

         void CloseDoor(){
        doorStatus=false;
        print("open works!");
        transform.DORotateQuaternion(new Quaternion(-0.707106829f,0,0,0.707106829f),3f);
        }
        
       

}
