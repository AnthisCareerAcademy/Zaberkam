using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    private float delay = .1f;
    private bool isColliding = false;
    private bool checking = false;

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Door"))
        {
            isColliding = true;

            if (!checking)
            {
                checking = true;
                StartCoroutine(CheckAfterDelay());
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Door"))
        {
            isColliding = false;
        }
    }

    private IEnumerator CheckAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        if (isColliding)
        {
            gameObject.SetActive(false);
        }

        checking = false;
    }
}
