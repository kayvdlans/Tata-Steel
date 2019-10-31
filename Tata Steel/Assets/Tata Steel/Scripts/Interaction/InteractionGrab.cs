using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionGrab : Interaction
{
    [SerializeField] private bool lockRotationOnGrab;

    private RigidbodyConstraints constraints;

    protected override void Initialize()
    {
        constraints = lockRotationOnGrab ? RigidbodyConstraints.FreezeRotation : RigidbodyConstraints.None;
    }

    public override void OnInteractionStart()
    {
        base.OnInteractionStart();
    }

    public override void OnInteractionEnd()
    {
        base.OnInteractionEnd();
    }
}
