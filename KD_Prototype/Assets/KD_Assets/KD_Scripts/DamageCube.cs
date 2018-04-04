using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCube : MonoBehaviour
{
	void Start ()
    {
        Invoke("DestroySelf", 2f);
	}
	
    void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
