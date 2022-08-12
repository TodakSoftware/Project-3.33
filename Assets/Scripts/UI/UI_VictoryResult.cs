using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class UI_VictoryResult : MonoBehaviourPunCallbacks
{

    [Header("Texts")]
    public TextMeshProUGUI teamText;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI redirectText;
    float redirectDuration = 5f;

    [Header("Backgrounds")]
    public GameObject humanWinBG;
    public GameObject ghostWinBG;


    [Space(10)]
    
    //the game object to spawn
    [Header("Hand Spawner--------------------------------")]
    
    [Header("The basics")]
    public GameObject SpawningImagePrefab;
    public int SpawnOffset;
    public GameObject ParentObject;
    [SerializeField]
    private int SpawnCurrentNumber;
    public int SpawnLimit;

    [Space]

    [Header("Spawn Frequencies (Unused)")]
    [Tooltip("The minimum amount of object to spawn in X")]
    public int SpawnFrequencyXMin;
    [Tooltip("The maximum amount of object to spawn in X")]
    public int SpawnFrequencyXMax;
    [Space]
    [Tooltip("The minimum amount of object to spawn in Y")]
    public int SpawnFrequencyYMin;
    [Tooltip("The maximum amount of object to spawn in Y")]
    public int SpawnFrequencyYMax;

    [Space]

    [Header("Positionings")]
    [Tooltip("The minimum amount of object to spawn in X")]
    public int SpawnRowNumber;
    [Tooltip("The maximum amount of object to spawn in X")]
    public int SpawnColumnNumber;



    [Space]

    [Header("Spawn Rotations")]
    [Tooltip("The amount of angles to rotate the image")]
    public int SpawnRotationMinRange;
    public int SpawnRotationMaxRange;

    [Space]

    [Header("Timings")]
    [Tooltip("How long to wait before the next spawn")]
    public float SpawnDelay;

    //
    void Start()
    {
        //initialising the hand spawner count
        SpawnCurrentNumber = 0;

        //double confirming if the backgrounds is disabled
        humanWinBG.SetActive(false);
        ghostWinBG.SetActive(false);

    }


    public void HumanWin()
    {
        teamText.color = new Color32(0, 0, 0, 255);
        teamText.text = "Human Victory!";
        messageText.text = "Escaped, for now.";
        Invoke("DelayRedirect",.3f);
        humanWinBG.SetActive(true);
        ghostWinBG.SetActive(false);
    }


    public void GhostWin(){
        teamText.color = new Color32(255, 255, 255, 255);
        teamText.text = "Ghost Victory!";
        messageText.text = "There is no escape.";
        Invoke("DelayRedirect",.3f);
        humanWinBG.SetActive(false);
        ghostWinBG.SetActive(true);
    }

    void DelayRedirect(){
        StartCoroutine(RedirectAfterEndGame());
    }

    IEnumerator RedirectAfterEndGame(){
        while(redirectDuration > 0){
            redirectText.text = "Redirect to main menu in "+redirectDuration;
            yield return new WaitForSeconds(1f);
            redirectDuration -= 1;
        }

        if(redirectDuration <= 0){
            //PhotonNetwork.LeaveRoom();
            if(photonView.IsMine){
                GameManager.instance.LeaveRoom();
            }
        }
    } // end RedirectAfterEndGame




    //moved the code from hand thing here
    //simple mode first
    public void SpawnTheImage()
    {
        Debug.Log("Screen Resolution: " + Screen.currentResolution);
        while (SpawnLimit != SpawnCurrentNumber)
        {
            Instantiate(SpawningImagePrefab, new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), 0), Quaternion.Euler(0, 0, Random.Range(SpawnRotationMinRange, SpawnRotationMaxRange)), ParentObject.transform);
            SpawnCurrentNumber++;
        }

        SpawnCurrentNumber = 0;
    }

    //delay version
    IEnumerator SpawnTheImageDelayVersionIEnumerator(int TotalImages, int RowNumberHere, float SpawnDelayHere)
    {
        //for checking rows
        for (int CurrentRowNumberHere = 0; CurrentRowNumberHere < RowNumberHere; CurrentRowNumberHere++)
        {
            //for spawning
            for (int i = 0; i < (TotalImages / RowNumberHere); i++)
            {
                Instantiate(SpawningImagePrefab, new Vector3(Random.Range(0, Screen.width), Random.Range(CurrentRowNumberHere * (Screen.height / RowNumberHere), (CurrentRowNumberHere + 1) * (Screen.height / RowNumberHere)), 0), Quaternion.Euler(0, 0, Random.Range(SpawnRotationMinRange, SpawnRotationMaxRange)), ParentObject.transform);
                yield return new WaitForSeconds(SpawnDelayHere);
            }

        }


    }

    //(Screen.height / SpawnRowNumber) = 

    public void SpawnTheImageDelayVersion()
    {

        StartCoroutine(SpawnTheImageDelayVersionIEnumerator(SpawnLimit, SpawnRowNumber, SpawnDelay));

    }

}
