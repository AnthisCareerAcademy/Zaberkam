using UnityEngine;
using System.Collections;

public class FireBall : MonoBehaviour
{
    public float speed = 40.0f;
    public float timeOfSurvival = 5f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(TimeOFSurvival());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
    }
    private IEnumerator TimeOFSurvival()
    {
        yield return new WaitForSeconds(timeOfSurvival);
        Destroy(gameObject);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            return;
        }
        
        Destroy(gameObject);
    }
}

