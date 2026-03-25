using UnityEngine;

public class Explosion : Attack
{
    // A lot of functions were reused here, so it's just an extension of attack now :D
    [SerializeField] ParticleSystem explosionParticles;
    
    public void OnDestroy()
    {
        DoAttack();

        ParticleSystem particles = Instantiate(explosionParticles, transform.position, Quaternion.identity);
        particles.Play();
    }
}
