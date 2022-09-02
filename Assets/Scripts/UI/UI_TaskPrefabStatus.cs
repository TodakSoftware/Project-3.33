using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TaskPrefabStatus : MonoBehaviour
{
    public GameObject statusImage;
    public Sprite iconComplete;
    public Sprite iconIncomplete;
    public bool isComplete;
    // Start is called before the first frame update
    void Start()
    {
        ChangeStatusImage(isComplete);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ChangeStatusImage(bool isCompleteHere)
    {
        if(isCompleteHere)
        {
            statusImage.GetComponent<Image>().sprite = iconComplete;
        }
        else
        {
            statusImage.GetComponent<Image>().sprite = iconIncomplete;
        }
    }

    public void SetToComplete()
    {
            statusImage.GetComponent<Image>().sprite = iconComplete;
    }

    public void SetToIncomplete()
    {
        statusImage.GetComponent<Image>().sprite = iconIncomplete;
    }
}
