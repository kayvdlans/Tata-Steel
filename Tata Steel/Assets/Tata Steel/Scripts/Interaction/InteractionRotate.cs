using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable), typeof(Rigidbody))]
public class InteractionRotate : MonoBehaviour
{
    [SerializeField] private float maxAngularVelocity = 1f;
    [SerializeField] private uint rotationLock = 0;
    [SerializeField] private float damping = 1f;

    private Interactable interactable = null;
    private Rigidbody rb = null;

    private Transform initialAttachPoint = null;

    private const float DELTA_MAGIC = 1f;

    private float currentAngle = 0;
    private float previousAngle = 0;

    [SerializeField] private float actualAngle = 0;
    [SerializeField] private int halfRotations = 0;

    private bool clockwise = true;
    private bool backwards = false;

    private void Start()
    {
        interactable = GetComponent<Interactable>();
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity;
    }

    private void FixedUpdate()
    {
        if (interactable.isInteracting)
        {
            Vector3 positionDelta = (interactable.hand.transform.position - initialAttachPoint.position) * DELTA_MAGIC;
            //positionDelta /= damping;

            rb.AddForceAtPosition(positionDelta, initialAttachPoint.position, ForceMode.VelocityChange);
        }

        if (rb.angularVelocity.x < -0.01f && actualAngle <= 0)
        {
            rb.angularVelocity = Vector3.zero;
            actualAngle = 0;
            halfRotations = 0;
            transform.rotation = Quaternion.identity;
        }

        if (rb.angularVelocity.x > 0.01f && actualAngle >= (360 * rotationLock))
        {
            rb.angularVelocity = Vector3.zero;
            actualAngle = (360 * rotationLock);
            halfRotations = (int)rotationLock;
            transform.rotation = Quaternion.Euler(360 * rotationLock, 0, 0);
        }

        currentAngle = Quaternion.Angle(Quaternion.identity, transform.rotation);

        clockwise = rb.angularVelocity.x >= 0;

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

        actualAngle = halfRotations * 180 + (backwards ? (180 - currentAngle) : currentAngle);
        previousAngle = currentAngle;
    }

    public void OnInteractionStart()
    {
        if (initialAttachPoint == null)
        {
            initialAttachPoint = new GameObject(string.Format("[{0}] InitialAttachPoint", this.gameObject.name)).transform;
            initialAttachPoint.position = interactable.hand.transform.position;
            initialAttachPoint.rotation = interactable.hand.transform.rotation;
            initialAttachPoint.localScale = Vector3.one * 0.25f;
            initialAttachPoint.parent = transform;
        }
    }

    public void OnInteractionEnd()
    {
        if (initialAttachPoint != null)
            Destroy(initialAttachPoint.gameObject);
    }
}
