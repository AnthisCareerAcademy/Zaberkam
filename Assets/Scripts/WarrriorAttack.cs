using UnityEngine;
using System.Collections;

public class WarrriorAttack : MonoBehaviour, IDamageDealer
{
    public float damage = 50f;
    public float Damage => damage;

    private Transform swordUp;
    private Transform swordDown;
    private bool isAttacking;

    void Start()
    {
        swordUp = transform.Find("SwordUp");
        swordDown = transform.Find("SwordDown");
        isAttacking = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(AttackRoutine(0.1f));
        }
    }
    private IEnumerator AttackRoutine(float cooldownTimer)
    {
        isAttacking = true;
        swordUp.gameObject.SetActive(false);
        swordDown.gameObject.SetActive(true);

        yield return new WaitForSeconds(cooldownTimer);

        swordDown.gameObject.SetActive(false);
        swordUp.gameObject.SetActive(true);
        isAttacking = false;
    }
}