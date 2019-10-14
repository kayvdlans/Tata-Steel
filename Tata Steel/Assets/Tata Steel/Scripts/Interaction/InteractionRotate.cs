﻿using UnityEngine;

[RequireComponent(typeof(Interactable), typeof(Rigidbody))]
public class InteractionRotate : MonoBehaviour
{
    [Tooltip("Changing the axis the object rotates around in this script changes the rigidbody's " +
        "constraints for you, so you don't have to manually do it every time you change the axis " +
        "of the rotation you want.")]
    [SerializeField] private MathHelper.Axis rotationAxis;

    [Tooltip("If this box is checked the rotation of the object will instantly stop as soon as " +
        "the object is no longer held")]
    [SerializeField] private bool lockRotationOnInteractionEnd;

    [Tooltip("The amount of full rotations of the valve it will take before it prevents you " +
        "from rotating any further")]
    [SerializeField] private uint rotationLock = 0;

    [SerializeField] private float maxAngularVelocity = 1f;

    private Interactable interactable = null;
    private Rigidbody rb = null;
    private Transform initialAttachPoint = null;

    //Works as a replacement for mass since the force mode of our rotation is VelocityChange,
    //which does not get affected by mass.
    private const float DELTA_MAGIC = 1f;  

    private float currentAngle = 0;
    private float previousAngle = 0;
    private int halfRotations = 0;
    private bool clockwise = true;
    private bool backwards = false;

    private RigidbodyConstraints rotationConstraints;

    public float ActualAngle { get; private set; } = 0;

    private void Start()
    {
        interactable = GetComponent<Interactable>();
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity;

        //Sets the constraints of the rigidbody based on the rotation axis.
        rotationConstraints = rotationAxis == MathHelper.Axis.X 
            ? RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ : rotationAxis == MathHelper.Axis.Y 
            ? RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ : rotationAxis == MathHelper.Axis.Z 
            ? RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY : RigidbodyConstraints.FreezeRotation;
        rb.constraints = RigidbodyConstraints.FreezePosition | rotationConstraints;
    }

    private void FixedUpdate()
    {
        if (interactable.isInteracting && interactable.closestHand)
        {
            //Checks the change in position from the initial point the player started interacting 
            //with the object and the current position of the hand.
            Vector3 positionDelta = (interactable.closestHand.transform.position - initialAttachPoint.position) * DELTA_MAGIC;

            //Add force based on the change of position.
            rb.AddForceAtPosition(positionDelta, initialAttachPoint.position, ForceMode.VelocityChange);
        }

        //If you're trying to rotate counter-clockwise while the angle is already at 0, 
        //it will keep resetting to prevent you from rotating it any further.
        if (rb.angularVelocity.x < -0.01f && ActualAngle <= 0)
        {
            rb.angularVelocity = Vector3.zero;
            ActualAngle = 0;
            halfRotations = 0;
            transform.rotation = Quaternion.identity;
        }

        //Same like previous, but the other way around.
        if (rb.angularVelocity.x > 0.01f && ActualAngle >= (360 * rotationLock))
        {
            rb.angularVelocity = Vector3.zero;
            ActualAngle = (360 * rotationLock);
            halfRotations = (int)rotationLock * 2;
            transform.rotation = Quaternion.Euler(360 * rotationLock, 0, 0);
        }

        currentAngle = Quaternion.Angle(Quaternion.identity, transform.rotation);

        clockwise = rb.angularVelocity.x >= 0;

        //If there is movement, check whether a half rotation should be added.
        //This is to make sure the actual rotation of the object is being documented,
        //Since normally the current angle only fluctuates between 0 and 180.
        if (Mathf.Abs(rb.angularVelocity.x) > 0.01f)
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

        //Calculate the actual angle based on half rotations and the current angle.
        ActualAngle = halfRotations * 180 + (backwards ? (180 - currentAngle) : currentAngle);
        previousAngle = currentAngle;
    }

    public void OnInteractionStart()
    {
        //Create an empty object at the position of the hand as soon as you start interacting with the object.
        //Used to calculate the distance traveled and calculate the rotation.
        if (initialAttachPoint == null)
        {
            initialAttachPoint = new GameObject(string.Format("[{0}] InitialAttachPoint", this.gameObject.name)).transform;
            initialAttachPoint.position = interactable.closestHand.transform.position;
            initialAttachPoint.rotation = interactable.closestHand.transform.rotation;
            initialAttachPoint.localScale = Vector3.one * 0.25f;
            initialAttachPoint.parent = transform;
        }

        if (lockRotationOnInteractionEnd)
            rb.constraints = RigidbodyConstraints.FreezePosition | rotationConstraints;
    }

    public void OnInteractionEnd()
    {
        if (initialAttachPoint != null)
            Destroy(initialAttachPoint.gameObject);

        if (lockRotationOnInteractionEnd)
            rb.constraints = RigidbodyConstraints.FreezeAll;
    }
}
