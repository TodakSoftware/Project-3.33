using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class UI_LoadingShaderTester : MonoBehaviour
{

    //debugger stuff
    public TextMeshProUGUI debuggerText;

    // Start is called before the first frame update
    void Start()
    {
        debuggerText.text = "Current Render Pipeline: " + UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset.GetType().Name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}