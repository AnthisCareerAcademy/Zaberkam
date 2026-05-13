using UnityEngine;

public class EmptyAttack : AttackTemplate
{
    public override void DoAttack(int bonus, float multiplier = 1F, Vector3? direction = null)
    {
        // It doesn't do anything...
    }
}
