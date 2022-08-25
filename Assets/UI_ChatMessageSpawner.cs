using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_ChatMessageSpawner : MonoBehaviour
{

    [Header("Prefabs and pointers")]
    public GameObject IncomingMessagePrefab;
    public GameObject OutgoingMessagePrefab;
    public GameObject ParentObject;

    [Header("Colors")]
    public Color Player01Color;
    public Color Player02Color;
    public Color Player03Color;

    [Header("Colors")]
    public GameObject ChatInputField;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //image.GetComponent<Image>().color = new Color32(255,255,225,100);

    public void CreateIncomingMessageSimple()
    {
        GameObject instance = Instantiate(IncomingMessagePrefab, ParentObject.transform);
    }

    public void CreateOutgoingMessageSimple()
    {
        GameObject instance = Instantiate(OutgoingMessagePrefab, ParentObject.transform);
    }

    public void CreateIncomingMessage(string PlayerName, string PlayerMessage, Color PlayerColor)
    {
        GameObject instance = Instantiate(IncomingMessagePrefab, ParentObject.transform);
        instance.GetComponent<UI_Message_Init>().playerNameString = PlayerName;
        instance.GetComponent<UI_Message_Init>().playerMessageString = PlayerMessage;
        instance.GetComponent<UI_Message_Init>().playerColor = PlayerColor;
    }

    public void CreateOutgoingMessage(string PlayerName, string PlayerMessage, Color PlayerColor)
    {
        GameObject instance = Instantiate(OutgoingMessagePrefab, ParentObject.transform);
        instance.GetComponent<UI_Message_Init>().playerNameString = PlayerName;
        instance.GetComponent<UI_Message_Init>().playerMessageString = PlayerMessage;
        instance.GetComponent<UI_Message_Init>().playerColor = PlayerColor;
    }

    /// <summary>
    /// Please edit this later to pull name and color from the player info or something
    /// </summary>
    /// <param name="PlayerMessage">The message</param>
    public void CreateOutgoingMessageUsingInputField()
    {
        GameObject instance = Instantiate(OutgoingMessagePrefab, ParentObject.transform);
        instance.GetComponent<UI_Message_Init>().playerNameString = "Me";
        instance.GetComponent<UI_Message_Init>().playerMessageString = ChatInputField.GetComponent<TMP_InputField>().text;
        instance.GetComponent<UI_Message_Init>().playerColor = Player01Color;

        //clears the text
        ChatInputField.GetComponent<TMP_InputField>().text = "";
    }
}
