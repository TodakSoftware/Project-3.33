using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_TaskMain : MonoBehaviour
{
    public Animator taskAnimator;
    public float waitTime;
    public bool isShowing;
    public GameObject taskStatusPrefab;
    public GameObject TaskStatusParent;

    // Start is called before the first frame update
    void Start()
    {
        taskAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Tab))
        {
            StartCoroutine(AnimateTaskWindow());
        }
    }

    IEnumerator AnimateTaskWindow()
    {
        if (!isShowing)
        {
            isShowing = true;
            ShowTaskWindow();
            yield return new WaitForSeconds(waitTime);
            HideTaskWindow();
            //isshowing is then changed in UI_TaskExit animation event
        }
    }


    public void ShowTaskWindow()
    {
        taskAnimator.Play("UI_TaskEntry");
    }


    public void HideTaskWindow()
    {
        taskAnimator.Play("UI_TaskExit");
    }

    public void SetIsShowingToFalse()
    {
        isShowing = false;
    }


    public void GenerateTask()
    {

    }

}
