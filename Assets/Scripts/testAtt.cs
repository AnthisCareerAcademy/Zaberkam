using UnityEngine;

public class testAtt : MonoBehaviour
{
    public float stat = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Test");
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            AttributeManager am = other.gameObject.GetComponent<AttributeManager>();
            if (am != null)
            {
                am.UpdateSpeed(stat);
            }
            Destroy(gameObject);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
