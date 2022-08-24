using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSelfDestroy : MonoBehaviour
{
    public float durationDestroy;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(durationDestroy);
        Destroy(this.gameObject);
    }

    

}
