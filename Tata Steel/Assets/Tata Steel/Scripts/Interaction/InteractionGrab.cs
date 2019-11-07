using System;
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
    private RigidbodyConstraints initialConstraints = RigidbodyConstraints.None;
    private bool initialKinematicState = false;
    private bool initialGravity = false;

    protected override void Initialize()
    {
        initialParent = transform.parent;
        initialConstraints = rb.constraints;
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

    protected override void OnInteractionStart()
    {
        base.OnInteractionStart();

        rb.isKinematic = makeKinematicOnGrab;
        rb.useGravity = false;

        initialRotation = transform.rotation;

        transform.SetParent(interactable.ClosestHand.transform);
        transform.localPosition = Vector3.zero;

        if (!snapPosition)
            transform.position += transform.position - initialAttachPoint.position;

        if (lockRotationOnGrab)
        {
            if (makeKinematicOnGrab)
                transform.rotation = initialRotation;
            else
                rb.constraints = RigidbodyConstraints.FreezeRotation; //TODO: more precision, freeze per axis instead.
        }

    }

    protected override void OnInteractionEnd()
    {
        transform.SetParent(initialParent);

        base.OnInteractionEnd();

        rb.isKinematic = initialKinematicState;
        rb.useGravity = initialGravity;
        rb.constraints = initialConstraints;

        //TODO: calculate velocity yourself instead of OVR, since it doesn't work too well.
        rb.velocity = OVRInput.GetLocalControllerVelocity(interactable.Controller);
        rb.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(interactable.Controller) *  Mathf.Deg2Rad;
    }
}
