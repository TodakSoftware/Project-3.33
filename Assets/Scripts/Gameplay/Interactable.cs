using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{

    public UnityEvent onInteract;
    public E_InteractType interactType;
    public E_ButtonType buttonType;
    public float holdDuration;
    [HideInInspector] public int id;
    
    void Start()
    {
        id = Random.Range(0, 999999);
    }
}
