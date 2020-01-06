using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ElevatorButton : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void OnTriggerEnter(Collider collider)
    {
        //make sure its a hand that is hitting the button
        if (collider.GetComponentInParent<HandCollider>() != null)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !animator.IsInTransition(0))
            {
                animator.SetTrigger("ElevatorButton");
            }
        }
    }
}
