using UnityEngine;

public class RangerClass : ClassTemplate
{
    [Header("Arrow Rain Settings")]
    [SerializeField] Vector3 arrowRainCenter;
    [SerializeField] float arrowRainAngle = 60;
    [SerializeField] float radius = 3f;
    [SerializeField] int minArrows = 50;
    [SerializeField] int maxArrows = 70;
    private bool zooming;
    
    public override void Update()
    {
        base.Update();

        if (!zooming)
        {
            ChangeFOV();
        }

        zooming = false;
    }
    // Primary is just shooting an arrow -- nothing special.
    
    protected override void DoSecondary()
    {
        // Zoom in to help with aiming.
        zooming = true;
        ChangeFOV(30, 2f);
        
    }
    
    // All values for the first ability (charge shot) are set in the inspector.
    
    protected override void DoSecondAbility()
    {
        // Shoot three arrows in different directions.
        Quaternion left = transform.rotation * Quaternion.Euler(0, -30, 0);
        Quaternion right = transform.rotation * Quaternion.Euler(0, 30, 0);
        attackHandlers.secondAbility.DoAttack(direction: left.eulerAngles);
        attackHandlers.secondAbility.DoAttack(direction: transform.eulerAngles);
        attackHandlers.secondAbility.DoAttack(direction: right.eulerAngles);
    }

    // Third attack (shooting a bomb arrow) is also handled by scripts.
    
    protected override void DoFourthAbility()
    {
        Quaternion angle = Quaternion.Euler(arrowRainAngle, transform.eulerAngles.y, 0);
        
        for (int i = 0; i < Random.Range(minArrows, maxArrows); i++)
        {
            Vector3 randomDirection = new Vector3(Random.Range(-radius, radius), 0, Random.Range(-radius, radius));
            attackHandlers.fourthAbility.transform.localPosition = arrowRainCenter + randomDirection;
            attackHandlers.fourthAbility.DoAttack(direction: angle.eulerAngles);
        }
    }
}
