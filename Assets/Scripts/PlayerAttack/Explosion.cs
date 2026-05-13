using UnityEngine;

public class Explosion : Attack
{
    // A lot of functions were reused here, so it's just an extension of attack now :D
    [SerializeField] ParticleSystem explosionParticles;
    [SerializeField] Vector2 randomMultiplier;
    
    public void OnDestroy()
    {
        DoAttack(multiplier: Random.Range(randomMultiplier.x, randomMultiplier.y));

        ParticleSystem particles = Instantiate(explosionParticles, transform.position, Quaternion.identity);
        particles.Play();
    }
}
