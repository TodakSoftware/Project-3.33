using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobilePhone : MonoBehaviour
{
    public Canvas canvas;
    public PlayerMovement player; // Got this from PlayerMovement (SetupPlayerMesh()) after spawn HP
    // Start is called before the first frame update
    void Start()
    {
        canvas.worldCamera = player.cam;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
