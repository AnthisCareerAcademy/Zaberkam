using UnityEngine;

// Each class should inherit from the ClassTemplate.
public class DefaultClass : ClassTemplate
{
    // You can define overrides for the actions or just have the attacks happen automatically.
    // base.Update() must be called first because it handles pausing (if you intend to override Update).
    protected override void DoPrimary()
    {
        print("Primary");
        
        base.DoPrimary();
    }
    
    protected override void DoSecondary()
    {
        print("Secondary");
        
        base.DoSecondary();
    }
    
    protected override void DoFirstAbility()
    {
        print("First Ability");
        
        base.DoFirstAbility();
    }
    
    protected override void DoSecondAbility()
    {
        print("Second Ability");
        
        base.DoSecondAbility();
    }

    protected override void DoThirdAbility()
    {
        print("Third Ability");
        
        base.DoThirdAbility();
    }
    
    protected override void DoFourthAbility()
    {
        print("Fourth Ability");
        
        base.DoFourthAbility();
    }
}
