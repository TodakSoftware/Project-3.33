using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEngine.Rendering.HighDefinition
{
public class ClearDepth : MonoBehaviour
{
    public List<GameObject> originalMaterial = new List<GameObject>();
    
    void OnEnable(){
        foreach(var mat in originalMaterial){
            mat.SetActive(false);
        }
    }

    void OnDisable(){
        foreach(var mat in originalMaterial){
            mat.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponent<HDAdditionalCameraData>().clearDepth){
            GetComponent<HDAdditionalCameraData>().clearDepth = false;
        }
    }
}
}