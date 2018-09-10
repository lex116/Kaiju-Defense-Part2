using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempExplosionScript : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
        Invoke("KillSelf", 4f);
	}
	
	void KillSelf()
    {
        Destroy(this.gameObject);
    }
}
