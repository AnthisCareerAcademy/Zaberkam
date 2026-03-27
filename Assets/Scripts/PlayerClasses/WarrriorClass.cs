using UnityEngine;

public class WarriorClass : ClassTemplate
{
    [SerializeField] float lungeSpeed = 10f;
    [SerializeField] float leapHeight = 10f;

    public override void Start()
    {
        base.Start();

        lungeSpeed *= scale;
        leapHeight *= scale;
    }
    
    // Nothing special for the primary attack. Basic sword thrust.

    protected override void DoSecondary()
    {
        // Thrust out a shield to deal light damage and prevent all incoming damage for 0.2 sec.
        StartCoroutine(Invincibility(0.2f));

        base.DoSecondary();
    }

    protected override void DoFirstAbility()
    {
        // Lunge forward to deal medium damage.
        
        // DeltaTime isn't needed because it's for one frame.
        StartCoroutine(Dash(transform.forward, 0.2f, lungeSpeed));

        base.DoFirstAbility();
    }

    protected override void DoSecondAbility()
    {
        // Spin around and deal damage to surrounding enemies.
        
        // This is to prevent the player attacking itself. You will still hit nearby players, though.
        StartCoroutine(Invincibility(0.1f));
        
        base.DoSecondAbility();
    }

    protected override void DoThirdAbility()
    {
        // Rally yourself and gain 20% critical chance for 5 seconds.
        StartCoroutine(CritChanceUp(5, 0.2f));

        base.DoThirdAbility();
    }

    protected override void DoFourthAbility()
    {
        // Jump into the air and slam your sword into nearby enemies.
        Velocity.y = leapHeight;
        
        base.DoFourthAbility();
    }
}
