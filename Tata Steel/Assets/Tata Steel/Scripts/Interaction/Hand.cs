using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private Material selectionMaterial;
    [SerializeField] private float waitTime;

    public OVRInput.Controller Controller { get; private set; }
    public bool IsInteracting { get; set; }
    public Material SelectionMaterial { get { return selectionMaterial; } }

    private List<Interactable> interactables = new List<Interactable>();
    private Interactable closestInteractable = null;

    private void Awake()
    {
        bool left = GetComponent<OvrAvatarHand>().isLeftHand;
        
        Controller = left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;

        Collider c = GetComponent<Collider>();
        if (c) DestroyImmediate(c);

        c = gameObject.AddComponent<BoxCollider>();

        (c as BoxCollider).center = new Vector3(-0.025f, -0.025f, -0.025f);
        (c as BoxCollider).size = new Vector3(0.05f, 0.05f, 0.05f);

        StartCoroutine(CheckForClosestInteractable(waitTime));
    }

    private IEnumerator CheckForClosestInteractable(float waitTime)
    {
        while (true)
        {
            if (interactables.Count == 0)
                closestInteractable = null;

            if (interactables.Count > 0 && !IsInteracting)
            {
                float closestDistance = float.MaxValue;

                foreach(Interactable interactable in interactables)
                {
                    float distance = Vector3.Distance(transform.position, interactable.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestInteractable = interactable;
                    }
                }
            }

            if (closestInteractable != null)
            {
                closestInteractable.CanInteract = true;
                closestInteractable.Controller = Controller;
            }

            yield return new WaitForSeconds(waitTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();

        if (interactable &&
            !interactables.Contains(interactable))
        { 
            interactable.CanInteract = false;
            interactable.ClosestHand = this;
            interactable.Controller = OVRInput.Controller.LTouch;

            interactables.Add(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();

        if (interactable &&
            interactables.Contains(interactable))
        {
            if (interactable.Controller.Equals(Controller))
            {
                interactable.CanInteract = false;
                interactable.ClosestHand = null;
                interactable.Controller = OVRInput.Controller.None;
            }

            interactables.Remove(interactable);
        }
    }
}
