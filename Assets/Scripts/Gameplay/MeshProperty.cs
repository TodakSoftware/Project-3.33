using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MeshProperty : MonoBehaviour
{
    public GameObject itemHandlerGO;
    [HideInInspector] public GameObject toLookAt;
    
    [Title("Procedural Animations")]
    public bool enableProcedural = true;
    public GameObject chestTarget;



    void Update(){
        if(enableProcedural){
            if(toLookAt != null){
                chestTarget.transform.position = new Vector3(chestTarget.transform.position.x, toLookAt.transform.position.y,chestTarget.transform.position.z);
            }
        }
    }
}
