using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawners : MonoBehaviour
{
    public GameObject spawnObject;
    public Transform spawnLocation;
    
    // Start is called before the first frame update

    

      void Update()
    {
         
       
        if(Input.GetKeyDown(KeyCode.E)){


          spawnMaker();
        }

    }


    // Update is called once per frame
    void spawnMaker()
    {
        {

            Instantiate(spawnObject,spawnLocation.position,Quaternion.identity);
            
          

        }

    }

     

    
}
    





      

       
        
       
    
        



