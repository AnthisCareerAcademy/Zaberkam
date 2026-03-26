using UnityEngine;
using System.Collections;
public class PlayerAttack : MonoBehaviour
{
    [SerializeField] public float characterAtk = 20f;
    public GameObject fireBall;
    private float cooldownTimer;
    private bool canShoot = true;
    public Transform firePoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canShoot)
        {
            FireBallAttack();
        }
    }
    IEnumerator Cooldown(float cooldownTimer)
    {
        canShoot = false;
        yield return new WaitForSeconds(cooldownTimer);
        canShoot = true;
    }
    private void FireBallAttack()
    {
        Instantiate(fireBall, firePoint.position, firePoint.rotation);
        StartCoroutine(Cooldown(2.5f));
    }

}
