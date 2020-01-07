﻿using UnityEngine;
using Valve.VR.InteractionSystem;

public class RotateValve : MonoBehaviour
{
    private Rigidbody rb;
    private CustomInteraction customInteraction;
    private Hand hand;
    private Transform initialAttachPoint;

    private bool isInteracting = false;

    [SerializeField] private MathHelper.Axis rotationAxis;
    [SerializeField] [Range(0, 180)] private float detachAngle = 45;
    [SerializeField] private bool lockRotationOnInteractionEnd;
    [SerializeField] private uint rotationLock = 0;
    [SerializeField] private float maxAngularVelocity = 1f;
    [SerializeField] private bool inverseAngularVelocity = true;

    private float currentAngle = 0;
    private float previousAngle = 0;
    [SerializeField] private int halfRotations = 0;
    private bool clockwise = true;
    private bool backwards = false;

    private RigidbodyConstraints rotationConstraints;
    private Quaternion initialRotation;

    public float ActualAngle { get; private set; } = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        customInteraction = GetComponent<CustomInteraction>();

        customInteraction.onStartInteraction.AddListener(CreateAttachPoint);
        customInteraction.onEndInteraction.AddListener(DestroyAttachPoint);

        customInteraction.interactionStarted += AssignHand;

        rb.maxAngularVelocity = maxAngularVelocity;

        //Sets the constraints of the rigidbody based on the rotation axis.
        rotationConstraints = rotationAxis == MathHelper.Axis.X
            ? RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ : rotationAxis == MathHelper.Axis.Y
            ? RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ : rotationAxis == MathHelper.Axis.Z
            ? RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY : RigidbodyConstraints.FreezeRotation;
        rb.constraints = RigidbodyConstraints.FreezePosition | rotationConstraints;

        initialRotation = transform.localRotation;
    }

    private void FixedUpdate()
    {
        if (isInteracting)
        {
            Vector3 dir = (hand.transform.position - initialAttachPoint.position).normalized;

            float dist = Mathf.Abs(
                Vector3.Angle(hand.transform.position, transform.position) * Mathf.Rad2Deg -
                Vector3.Angle(initialAttachPoint.position, transform.position) * Mathf.Rad2Deg);
            
            if (dist > detachAngle)
            {
                hand.DetachObject(gameObject, customInteraction.restoreOriginalParent);
                return;
            }

            rb.AddForceAtPosition(dir, initialAttachPoint.position, ForceMode.VelocityChange);
        }

        float aV = rotationAxis == MathHelper.Axis.X
                ? rb.angularVelocity.x : rotationAxis == MathHelper.Axis.Y
                ? rb.angularVelocity.y : rb.angularVelocity.z;
        float angularVelocity = inverseAngularVelocity ? -aV : aV;
        //LockRotation(angularVelocity);

        currentAngle = Quaternion.Angle(initialRotation, transform.localRotation);

        clockwise = angularVelocity >= 0;

        //If there is movement, check whether a half rotation should be added.
        //This is to make sure the actual rotation of the object is being documented,
        //Since normally the current angle only fluctuates between 0 and 180.
        if (Mathf.Abs(aV) > 0.01f)
        {
            UpdateHalfRotations();
        }

        //Calculate the actual angle based on half rotations and the current angle.
        ActualAngle = halfRotations * 180 + (backwards ? (180 - currentAngle) : currentAngle);
        previousAngle = currentAngle;
    }

    private void LockRotation(float angularVelocity)
    {
        //If you're trying to rotate counter-clockwise while the angle is already at 0, 
        //it will keep resetting to prevent you from rotating it any further.
        if (angularVelocity < -0.01f && ActualAngle <= 0)
        {
            rb.angularVelocity = Vector3.zero;
            ActualAngle = 0;
            halfRotations = 0;
            transform.localRotation = initialRotation;
        }

        //Same like previous, but the other way around.
        if (angularVelocity > 0.01f && ActualAngle >= (360 * rotationLock))
        {
            rb.angularVelocity = Vector3.zero;
            ActualAngle = (360 * rotationLock);
            halfRotations = (int)rotationLock * 2;
            transform.localRotation = Quaternion.Euler(360 * rotationLock, 0, 0);
        }
    }

    private void UpdateHalfRotations()
    {
        if (clockwise && !backwards && previousAngle > currentAngle)
        {
            halfRotations++;
            backwards = true;
        }
        else if (clockwise && backwards && currentAngle > previousAngle)
        {
            halfRotations++;
            backwards = false;
        }
        else if (!clockwise && !backwards && currentAngle > previousAngle)
        {
            halfRotations--;
            backwards = true;
        }
        else if (!clockwise && backwards && previousAngle > currentAngle)
        {
            halfRotations--;
            backwards = false;
        }
    }

    private void AssignHand(Hand hand)
    {
        this.hand = hand;
    }

    private void CreateAttachPoint()
    {
        if (initialAttachPoint == null)
        {
            initialAttachPoint = new GameObject(string.Format("[{0}] InitialAttachPoint", this.gameObject.name)).transform;
            initialAttachPoint.position = hand.transform.position;
            initialAttachPoint.rotation = hand.transform.rotation;
            initialAttachPoint.localScale = Vector3.one * 0.25f;
            initialAttachPoint.parent = transform;
        }

        if (lockRotationOnInteractionEnd)
            rb.constraints = RigidbodyConstraints.FreezePosition | rotationConstraints;

        isInteracting = true;
    }

    private void DestroyAttachPoint()
    {
        if (initialAttachPoint != null)
        {
            Destroy(initialAttachPoint.gameObject);
        }

        if (lockRotationOnInteractionEnd)
            rb.constraints = RigidbodyConstraints.FreezeAll;

        isInteracting = false;
    }
}
