using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    private const float kInteractableCheckTime = 0.2f;

    public OVRInput.Controller Controller { get; private set; }
    public bool IsInteracting { get; set; }
    public List<Interactable> Interactables { get; private set; } = new List<Interactable>();

    private Hand otherHand = null;
    private Interactable closestInteractable = null;

    private void Start ()
    {
        bool left = GetComponent<OvrAvatarHand>().isLeftHand;
        
        Controller = left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;

        Collider c = GetComponent<Collider>();
        if (c) DestroyImmediate(c);

        c = gameObject.AddComponent<BoxCollider>();
        c.isTrigger = true;

        (c as BoxCollider).center = new Vector3(0, 0, 0);
        (c as BoxCollider).size = new Vector3(0.2f, 0.2f, 0.2f);

        StartCoroutine(CheckForClosestInteractable(kInteractableCheckTime));
        StartCoroutine(FindOtherHand(0.1f));
    }

    private IEnumerator FindOtherHand(float waitTime)
    {
        while (otherHand == null)
        {
            yield return new WaitForSeconds(waitTime);
            Hand[] hands = FindObjectsOfType<Hand>();

            if (hands.Length > 1)
                otherHand = hands[0] == this ? hands[1] : hands[0];
        }
    }

    private IEnumerator CheckForClosestInteractable(float waitTime)
    {
        while (true)
        {
            if (Interactables.Count == 0)
                closestInteractable = null;

            float closestDistance = float.MaxValue;

            if (Interactables.Count > 0 && !IsInteracting)
            {

                foreach(Interactable interactable in Interactables)
                {
                    float distance = Vector3.Distance(transform.position, interactable.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestInteractable = interactable;
                    }
                }
            }

            if (closestInteractable != null && closestDistance < Vector3.Distance(closestInteractable.transform.position, otherHand.transform.position))
            {

                closestInteractable.CanInteract = true;
                closestInteractable.ClosestHand = this;
                closestInteractable.Controller = Controller;
            }

            yield return new WaitForSeconds(waitTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();

        if (interactable != null &&
            !Interactables.Contains(interactable))
        { 
            interactable.CanInteract = false;
            interactable.ClosestHand = this;
            interactable.Controller = Controller;

            Interactables.Add(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();

        if (interactable != null &&
            Interactables.Contains(interactable))
        {
            if (!otherHand.Interactables.Contains(interactable))
            {
                interactable.CanInteract = false;
                interactable.ClosestHand = null;
                interactable.Controller = OVRInput.Controller.None;
            }

            Interactables.Remove(interactable);
        }
    }
}
