using UnityEngine;

public class WaterEffect : MonoBehaviour
{
    public ParticleSystem wakeParticles;
    public Rigidbody boatRigidbody;
    public float minSpeedForWake = 2f;
    public float particleMultiplier = 10f;

    void Update()
    {
        if (boatRigidbody == null || wakeParticles == null) return;

        float speed = boatRigidbody.linearVelocity.magnitude;
        var emission = wakeParticles.emission;

        if (speed > minSpeedForWake)
        {
            emission.rateOverTime = speed * particleMultiplier;
            if (!wakeParticles.isPlaying) wakeParticles.Play();
        }
        else
        {
            emission.rateOverTime = 0;
            if (wakeParticles.isPlaying) wakeParticles.Stop();
        }
    }
}
