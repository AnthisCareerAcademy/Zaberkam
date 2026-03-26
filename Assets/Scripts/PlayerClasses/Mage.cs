using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.Netcode;

public class Mage : ClassTemplate
{
    [Header("Mana Settings")]
    [SerializeField] private Slider manaSlider;
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float mana = 100f;
    [SerializeField] private float manaRegenRate = 5f;


    // Disabled for the moment since they don't do much.
    // [Header("Abilities")]
    // public GameObject staff;
    // public GameObject tome;
    public GameObject magicMissile;
    [SerializeField] Transform magicMissileSpawnPoint;
    // public GameObject polymorph;

    public override void Start()
    {
        base.Start();
        
        mana = maxMana;
        UpdateManaUI();
    }

    public override void Update()
    {
        // Handle all attack functions at the end of the update function.
        base.Update();
        ManaRegen();
    }

    void ManaRegen()
    {
        if (mana < maxMana)
        {
            mana += manaRegenRate * Time.deltaTime;
            mana = Mathf.Clamp(mana, 0f, maxMana);
            UpdateManaUI();
        }
    }

    void UpdateManaUI()
    {
        if (manaSlider)
            manaSlider.value = mana / maxMana;
    }

    // ================= ABILITIES =================

    // Primary (M1) – Regenerates mana
    protected override void DoPrimary()
    {
        
        mana += 10f;
        mana = Mathf.Clamp(mana, 0f, maxMana);
        UpdateManaUI();
        base.DoPrimary();

        Debug.Log("Staff used");
    }

    // Secondary (M2) – Costs 20 mana
    protected override void DoSecondary()
    {
        if (!TrySpendMana(20f)) return;
        base.DoSecondary();

        Debug.Log("Tome used");
    }

    protected override void DoFirstAbility()
    {
        if (!TrySpendMana(30f)) return;
        // local rotation
        Instantiate(magicMissile, magicMissileSpawnPoint.position, magicMissileSpawnPoint.rotation);
        Debug.Log("Magic Missile cast");
    }

    protected override void DoSecondAbility()
    {
        if (!TrySpendMana(50f)) return;
        attackHandlers.secondAbility.DoAttack(direction: transform.eulerAngles);

        Debug.Log("Fireball cast");
    }

    protected override void DoThirdAbility()
    {
        if (!TrySpendMana(40f)) return;
        base.DoThirdAbility();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                Debug.Log("Enemy hit: " + hitCollider.gameObject.name);
                StartCoroutine(PolymorphEffect(hitCollider.gameObject));
                break;
            }
        }

        

        Debug.Log("Polymorph cast");
    }

    protected override void DoFourthAbility()
    {
        if (!TrySpendMana(67f)) return;
        StartCoroutine(InvisibilityEffect());

        Debug.Log("Invisibility cast");
    }

    bool TrySpendMana(float cost)
    {
        if (mana >= cost)
        {
            mana -= cost;
            UpdateManaUI();
            return true;
        }
        return false;
    }
    IEnumerator PolymorphEffect(GameObject enemy, float duration = 5f)
    {
        Debug.Log("Polymorphing " + enemy.name);
        Transform t = enemy.transform;
        Vector3 originalScale = t.localScale;
        t.localScale = originalScale * 0.5f;
        yield return new WaitForSeconds(duration);
        Debug.Log(enemy.name + " has returned to normal");
        t.localScale = originalScale;
    }
    IEnumerator InvisibilityEffect(float duration = 6.7f)
    {
        gameObject.tag = "Untagged";
        Debug.Log("You are now invisible");
        yield return new WaitForSeconds(duration);
        gameObject.tag = "Player";
        Debug.Log("You are now visible");
    }
}