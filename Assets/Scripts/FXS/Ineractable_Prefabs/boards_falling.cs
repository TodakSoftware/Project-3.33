using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boards_falling : MonoBehaviour
{

    Rigidbody boardHere;
    //public AudioSource audioboardfellHere;
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

        if (ghostHere.tag=="Ghost")
        
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