using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionInstantRotation : Interaction
{
    [SerializeField]
    [Range(1, 180)]
    private float rotationAmount = 5;


    [SerializeField]
    private MathHelper.Axis axis;

    private Quaternion initialRotation = Quaternion.identity;
    private Vector3 axisVector = Vector3.zero;
    private float axisDirection = 0f;
    private float rotationLock = float.MaxValue;

    public float CurrentAngle { get; private set; } = 0f;
    public PressureBuildup Buildup { get; set; } = null;

    protected override void Initialize()
    {
        if (Buildup)
            rotationLock = Buildup.AngleToLock;

        initialRotation = transform.rotation;

        switch (axis)
        {
            case MathHelper.Axis.X:
                axisVector = Vector3.right;
                break;
            case MathHelper.Axis.Y:
                axisVector = Vector3.up;
                break;
            case MathHelper.Axis.Z:
                axisVector = Vector3.forward;
                break;
        }
    }

    public override void OnInteractionStart()
    {
        base.OnInteractionStart();

        Vector3 vDir = (initialAttachPoint.position - transform.position).normalized;

        axisDirection = axis == MathHelper.Axis.X ? vDir.z : axis == MathHelper.Axis.Y ? vDir.x : vDir.y;
        Vector3 vRot = axisVector * (axisDirection < 0 ? -rotationAmount : rotationAmount);

        float aRot = axis == MathHelper.Axis.X ? vRot.x : axis == MathHelper.Axis.Y ? vRot.y : vRot.z;
        aRot *= -1f;

        if (CurrentAngle + aRot <= 0)
        {
            CurrentAngle = 0;
            transform.localRotation = initialRotation;
            return;
        }

        if (CurrentAngle + aRot >= rotationLock)
        {
            CurrentAngle = rotationLock;
            transform.localRotation = Quaternion.Euler(initialRotation.eulerAngles + vRot.normalized * rotationLock);
            return;
        }

        CurrentAngle += aRot;
        transform.Rotate(vRot, Space.Self);
    }
}
