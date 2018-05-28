using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    public LineRenderer tracer;

	void Start ()
    {
        Invoke("DestroySelf", 0.5f);
	}
	
    void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    public void SetOrigin(Vector3 origin)
    {
        tracer.SetPosition(0, this.transform.position);
        tracer.SetPosition(1, origin);
    }
}
