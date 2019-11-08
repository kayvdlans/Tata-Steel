using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionInstantRotation : Interaction
{
    [SerializeField] [Range (1, 180)]
    private float rotationAmount = 0;

    protected override void Initialize()
    {
    }

    protected override void OnInteractionStart()
    {
        base.OnInteractionStart();

        Debug.Log(Vector3.SignedAngle(initialAttachPoint.position, transform.position, Vector3.right));

        //if ()
    }
}
