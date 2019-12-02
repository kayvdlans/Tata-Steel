using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CustomInteraction))]
public abstract class Interaction : MonoBehaviour
{
    protected Interactable interactable { get; private set; } = null;
    protected Rigidbody rb { get; private set; } = null;
    protected Transform initialAttachPoint { get; private set; } = null;

    protected bool isInteracting
    {
        get
        {
            if (!interactable)
                return false;

            return interactable.IsInteracting && interactable.ClosestHand;
        }
    }

    protected void Start()
    {
        interactable = GetComponent<Interactable>();

        rb = GetComponent<Rigidbody>();

        Initialize();
    }

    protected abstract void Initialize();

    public virtual void OnInteractionStart()
    {
        //Create an empty object at the position of the hand as soon as you start interacting with the object.
        //Used to calculate the distance traveled and calculate the rotation as well as the offset while grabbing.
        if (initialAttachPoint == null)
        {
            initialAttachPoint = new GameObject(string.Format("[{0}] InitialAttachPoint", this.gameObject.name)).transform;
            initialAttachPoint.position = interactable.ClosestHand.transform.position;
            initialAttachPoint.rotation = interactable.ClosestHand.transform.rotation;
            initialAttachPoint.localScale = Vector3.one * 0.25f;
            initialAttachPoint.parent = transform;
        }
    }

    public virtual void OnInteractionEnd()
    {
        if (initialAttachPoint != null)
            Destroy(initialAttachPoint.gameObject);
    }
}
