using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionGrab : Interaction
{
    [SerializeField] private bool makeKinematicOnGrab = true;
    [SerializeField] private bool lockRotationOnGrab = false;   //does not work well (yet)
    [SerializeField] private bool snapPosition = false;         //does not work well (yet)

    private Transform initialParent = null;
    private Quaternion initialRotation = Quaternion.identity;
    private bool initialKinematicState = false;
    private bool initialGravity = false;

    protected override void Initialize()
    {
        initialParent = transform.parent;
        initialKinematicState = rb.isKinematic;
        initialGravity = rb.useGravity;
    }

    private void FixedUpdate()
    {
        if (isInteracting && lockRotationOnGrab)
        {
            if (makeKinematicOnGrab)
                transform.rotation = initialRotation;
        }
    }

    public override void OnInteractionStart()
    {
        base.OnInteractionStart();

        rb.isKinematic = makeKinematicOnGrab;
        rb.useGravity = false;

        initialRotation = transform.rotation;

        transform.SetParent(interactable.closestHand);
        transform.localPosition = Vector3.zero;

        if (!snapPosition)
            transform.position += transform.position - initialAttachPoint.position;

        if (lockRotationOnGrab)
        {
            if (makeKinematicOnGrab)
                transform.rotation = initialRotation;
            else
                rb.constraints = RigidbodyConstraints.FreezeRotation; //TODO: more precision, freeze each axis instead.
        }

    }

    public override void OnInteractionEnd()
    {
        transform.SetParent(initialParent);

        base.OnInteractionEnd();

        rb.isKinematic = initialKinematicState;
        rb.useGravity = initialGravity;
        rb.constraints = RigidbodyConstraints.None;
    }
}
