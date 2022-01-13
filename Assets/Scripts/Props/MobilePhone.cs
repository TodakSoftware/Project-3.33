using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobilePhone : MonoBehaviour
{
    public Canvas canvas;
    public GameObject flashLight;
    public PlayerMovement player; // Got this from PlayerMovement (SetupPlayerMesh()) after spawn HP
    // Start is called before the first frame update
    void Start()
    {
        canvas.worldCamera = player.cam;

        if(player.GetComponent<PlayerMovement>().spawnHandOnly){
            flashLight.transform.localPosition = new Vector3(0.029f, -4f, 0.012f);
        }
    }

    // Update is called once per frame
    public void ChangeLandscape(bool val){
        if(val){
            GetComponent<Animator>().SetBool("Landscape", true);
        }else{
            GetComponent<Animator>().SetBool("Landscape", false);
        }
    }
}
