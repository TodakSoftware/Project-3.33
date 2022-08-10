using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Human_Lose__Hand_Spawners : MonoBehaviour
{

    //the game object to spawn
    [Header("The basics")]
    public GameObject SpawningImagePrefab;
    public int SpawnOffset;
    public GameObject ParentObject;
    [SerializeField]
    private int SpawnCurrentNumber;
    public int SpawnLimit;

    [Header("Spawn Frequencies")]
    [Tooltip("The minimum amount of object to spawn in X")]
    public int SpawnFrequencyXMin;
    [Tooltip("The maximum amount of object to spawn in X")]
    public int SpawnFrequencyXMax;
    [Space]
    [Tooltip("The minimum amount of object to spawn in Y")]
    public int SpawnFrequencyYMin;
    [Tooltip("The maximum amount of object to spawn in Y")]
    public int SpawnFrequencyYMax;

    [Header("Spawn Rotations")]
    [Tooltip("The amount of angles to rotate the image")]
    public int SpawnRotationMinRange;
    public int SpawnRotationMaxRange;

    [Header("Timings")]
    [Tooltip("How long to wait before the next spawn")]
    public float SpawnDelay;

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log("Script is running!");

        //StartCoroutine(SpawnEnumerator());

        SpawnCurrentNumber = 0;

    }

    // Update is called once per frame
    void Update()
    {


    }


    //IEnumerator SpawnEnumerator()
    //{
    


    //}
       

    //simple mode first
    public void SpawnTheImage()
    {
        Debug.Log("Screen Resolution: "+ Screen.currentResolution);
        while (SpawnLimit != SpawnCurrentNumber)
        {
            Instantiate(SpawningImagePrefab, new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), 0), Quaternion.identity, ParentObject.transform);
            SpawnCurrentNumber++;
        }

        SpawnCurrentNumber = 0;
    }



}
