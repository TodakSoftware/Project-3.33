using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent onInteract;
    public Sprite interactIcon;
    public Vector2 iconSize;
    public int id;
    
    void Start()
    {
        id = Random.Range(0, 999999);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
