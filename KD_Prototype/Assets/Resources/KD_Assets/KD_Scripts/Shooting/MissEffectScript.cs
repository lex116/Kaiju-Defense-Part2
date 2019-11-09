using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissEffectScript : MonoBehaviour
{
    public LineRenderer tracer;
    ParticleSystem EffectParticleSystem;

	void Start ()
    {
        EffectParticleSystem = this.GetComponent<ParticleSystem>();

        float timer = EffectParticleSystem.main.duration;
        Invoke("DestroyTracer", 0.15f);
        Invoke("DestroySelf", timer);
    }

    void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    void DestroyTracer()
    {
        Destroy(tracer);
    }

    public void SetOrigin(Vector3 origin)
    {
        tracer.SetPosition(0, this.transform.position);
        tracer.SetPosition(1, origin);
    }
}
