using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class UI_SkillCooldown : MonoBehaviour
{
    public Image imageCooldown;
    public Image imageEdge;
    public TMP_Text textCooldown;
    public GameObject inputKeyImage;

    //cooldown stuff
    public bool isCooldown = false;
    public float cooldownTime = 10.0f;
    public float cooldownTimer = 0.0f;

    public UnityEvent stillInCooldownFunction;

    // Start is called before the first frame update
    void Start()
    {

        //hide since it's not started yet
        textCooldown.gameObject.SetActive(false);
        imageEdge.gameObject.SetActive(false);
        imageCooldown.fillAmount = 0.0f;


    }

    // Update is called once per frame
    void Update()
    {
        if(isCooldown)
        {
            ApplyCooldown();
        }

    }


    void ApplyCooldown()
    {

        //subtract time since last called
        cooldownTimer -= Time.deltaTime;

        if(cooldownTimer < 0.0f)
        {
            isCooldown = false;
            textCooldown.gameObject.SetActive(false);
            imageEdge.gameObject.SetActive(false);
            inputKeyImage.gameObject.SetActive(true);
            imageCooldown.fillAmount = 0.0f;
        }
        else
        {
            textCooldown.text = Mathf.RoundToInt(cooldownTimer).ToString();
            imageCooldown.fillAmount = cooldownTimer / cooldownTime;
            imageEdge.transform.localEulerAngles = new Vector3(0, 0, 360.0f * (cooldownTimer / cooldownTime));

        }

    }

    public void UseSkill()
    {

        if(isCooldown)
        {
            //code if still in cooldown (i.e show error or show notification syaying it still in cooldown
            stillInCooldownFunction.Invoke();
        }
        else
        {
            isCooldown = true;
            textCooldown.gameObject.SetActive(true);
            imageEdge.gameObject.SetActive(true);
            inputKeyImage.gameObject.SetActive(false);
            cooldownTimer = cooldownTime;
        }


    }
}
