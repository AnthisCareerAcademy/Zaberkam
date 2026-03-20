using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimation : MonoBehaviour
{
    [SerializeField] InputActionProperty gripAction;
    [SerializeField] InputActionProperty teleportAction;
    [SerializeField] InputActionProperty triggerAction;
    [SerializeField] Animator animator;

    void Update()
    {
        float gripValue = gripAction.action.ReadValue<float>();
        float teleportValue = teleportAction.action.ReadValue<float>();
        float triggerValue = triggerAction.action.ReadValue<float>();
        
        animator.SetFloat("Grip", gripValue);
        
        // This way, triggers or teleport actions make the hand point.
        if (teleportValue > triggerValue)
        {
            animator.SetFloat("Teleport", teleportValue);
        }
        else
        {
            animator.SetFloat("Teleport", triggerValue);
        }

    }
}
