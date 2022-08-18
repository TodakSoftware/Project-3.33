using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boards_falling : MonoBehaviour
{

    Rigidbody boardHere;
    public bool delayOn;
    //public AudioSource audioboardfellHere;
IEnumerator Starting(){

        yield return new WaitForSeconds(3f);
        delayOn=true;
     }
         // Start is called before the first frame update
    void Start()
    {
        boardHere=GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider ghostHere)
     
    {

        if (ghostHere.tag=="Ghost" && delayOn)
        
        {
            if(Input.GetKeyDown(KeyCode.E))
        
            {
                  boardHere.isKinematic=false;
                
                  
                  }       

                   if(Input.GetKey(KeyCode.E))
                     Debug.Log("cock");
                   
            //audioboardfellHere.Play();

        }

    }

}