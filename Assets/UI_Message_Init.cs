using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Message_Init : MonoBehaviour
{
    [Header("Objects")]
    public TextMeshProUGUI playerNameObject;
    public TextMeshProUGUI playerMessageObject;
    public GameObject profilePictureColorImage;

    [Header("Color")]
    public Color playerColor;

    [Header("Texts strings")]
    public string playerNameString;
    public string playerMessageString;


    // Start is called before the first frame update
    void Start()
    {

        playerNameObject.text = playerNameString;
        playerMessageObject.text = playerMessageString;

        playerNameObject.color = playerColor;
        profilePictureColorImage.GetComponent<Image>().color = playerColor;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
