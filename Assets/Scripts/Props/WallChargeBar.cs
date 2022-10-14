using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallChargeBar : MonoBehaviour
{
    [SerializeField] private Image _batterychargeSprite;

    private Camera _cam;

    void Start(){
        //_cam = Camera.main;
    }

   public void UpdateBatteryCharge(float wallChargerPercent, float currentCharge) {
        _batterychargeSprite.fillAmount = currentCharge / wallChargerPercent;
    }

    void Update() {
        //transform.rotation = Quaternion.LookRotation(transform.position - _cam.transform.position);
    }

}
