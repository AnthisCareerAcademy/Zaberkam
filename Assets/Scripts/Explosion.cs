public class Explosion : Attack
{
    // A lot of functions were reused here, so it's just an extension of attack now :D
    
    public void OnDestroy()
    {
        DoAttack();
    }
}
