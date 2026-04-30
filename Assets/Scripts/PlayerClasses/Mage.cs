using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Mage : ClassTemplate
{
    [Header("Mana Settings")]
    [SerializeField] private Slider manaSlider;
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float mana = 100f;
    [SerializeField] private float manaRegenRate = 5f;

    public override void Awake()
    {
        base.Awake();
        
        mana = maxMana;
        UpdateManaUI();
    }

    public override void Update()
    {
        // Handle all attack functions at the end of the update function.
        if (!IsOwner) return;
        ManaRegen();
        UpdateManaUI();
        base.Update();
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

    // Primary (M1) – Swing staff
    protected override void DoPrimary()
    {
        
        mana += 5f;
        mana = Mathf.Clamp(mana, 0f, maxMana);
        base.DoPrimary();
    }

    // Secondary (M2) – Burst of fire, costs 20 mana
    protected override void DoSecondary()
    {
        if (!TrySpendMana(20f)) return;
        base.DoSecondary();
    }

    protected override void DoFirstAbility()
    {
        if (!TrySpendMana(30f)) return;
        base.DoFirstAbility();
    }

    protected override void DoSecondAbility()
    {
        if (!TrySpendMana(50f)) return;
        base.DoSecondAbility();
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
    }

    protected override void DoFourthAbility()
    {
        if (!TrySpendMana(60f)) return;
        StartCoroutine(Invincibility(5f));
        StartCoroutine(InvisibilityEffect(5f));
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
        // TODO: make this polymorph enemies instead of shrinking them
        Transform t = enemy.transform;
        Vector3 originalScale = t.localScale;
        t.localScale = originalScale * 0.5f;
        yield return new WaitForSeconds(duration);
        t.localScale = originalScale;
    }
    IEnumerator InvisibilityEffect(float duration = 5f)
    {
        gameObject.tag = "Untagged";
        yield return new WaitForSeconds(duration);
        gameObject.tag = "Player";
    }
}