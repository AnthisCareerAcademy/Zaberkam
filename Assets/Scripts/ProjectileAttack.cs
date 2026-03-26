using UnityEngine;
using System.Collections;

public class ProjectileAttack : MonoBehaviour, IDamageDealer
{
    private PlayerAttack characterAtk;
    public float speed = 40.0f;
    public float timeOfSurvival;
    private float damage = 25f;
    public float Damage => damage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(TimeOFSurvival());
        Debug.Log(characterAtk);
        characterAtk = FindObjectOfType<PlayerAttack>();
        damage = characterAtk.characterAtk;
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            return;
        }
        
        Destroy(gameObject);
    }


    
    private IEnumerator TimeOFSurvival()
    {
        yield return new WaitForSeconds(timeOfSurvival);
        Destroy(gameObject);
        

    }
}
