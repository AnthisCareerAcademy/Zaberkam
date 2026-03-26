using UnityEngine;
using UnityEngine.UI;

public class AttributeManager : MonoBehaviour
{
    [SerializeField] public float baseDamage = 10f;
    [SerializeField] public float speed = 5f;
    [SerializeField] public float critChance = 0.1f;
    [SerializeField] float currentDamage;
    [SerializeField] public float currentSpeed;
    [SerializeField] float currentCritChance;

    void Start()
    {
        currentDamage = baseDamage;
        currentSpeed = speed;
        currentCritChance = critChance;
    }
    void UpdateDamage(float amount)
    {
        currentDamage += amount;
    }
    public void UpdateSpeed(float amount)
    {
        currentSpeed += amount;
    }
    public void UpdateCritChance(float amount)
    {
        currentCritChance += amount;
    }
    public void ResetAttributes()
    {
        currentDamage = baseDamage;
        currentSpeed = speed;
        currentCritChance = critChance;
    }
}