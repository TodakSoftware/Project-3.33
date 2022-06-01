//Camera spine controller

using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace scgFullBodyController
{
    public class CameraControlledIK : MonoBehaviourPunCallbacks
    {
        public Transform spineToOrientate;

        // Update is called once per frame
        void LateUpdate()
        {
            spineToOrientate.rotation = transform.rotation;
        }
    }
}
