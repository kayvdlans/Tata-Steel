using System.Collections;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(CustomInteraction), typeof(Rigidbody))]
public class RotateValve : MonoBehaviour
{
    private enum HapticProcessing
    {
        Constant,
        GraphBased,
        OneMinusVelocity
    }

    private Rigidbody rb;
    private CustomInteraction customInteraction;
    private Hand hand;
    private Transform initialAttachPoint;
    private Transform previousAttachPoint;
    private bool isInteracting = false;

    [Header("Haptic Feedback")]
    [SerializeField] private bool continuousHapticFeedback = false;
    [SerializeField] [Range(0, 320)] private float feedbackFrequency;
    [SerializeField] private HapticProcessing processingType;
    [SerializeField] private AnimationCurve amplitudeGraph;
    [SerializeField] [Range(0, 1)] private float minimumAmplitude;
    [SerializeField] [Range(0, 1)] private float feedbackAmplitude;

    [Header("Angular Velocity")]
    //Also used for haptic feedback as 1 - value
    [SerializeField] private AnimationCurve normalizedMaxAngularVelocity;
    [SerializeField] [Range(0.1f, 10f)] private float angularVelocityMultiplier;
    [SerializeField] private bool inverseAngularVelocity = true;

    [Header("Rotation")]
    [SerializeField] [Range(0, 180)] private float detachAngle = 45;
    [SerializeField] private MathHelper.Axis rotationAxis;
    [SerializeField] private bool lockRotationOnInteractionEnd;
    [SerializeField] private uint rotationLock = 0;

    private float currentAngle = 0;
    private float previousAngle = 0;
    private int halfRotations = 0;
    private bool clockwise = true;
    private bool backwards = false;
    private bool halfRotationPossible = true;
    private float halfRotationResetTimer = 1f;

    private RigidbodyConstraints rotationConstraints;
    private Quaternion initialRotation;

    public float ActualAngle { get; private set; } = 0;
    public float MaxAngle { get => rotationLock * 360; }
    public float AngleRatio { get => ActualAngle / MaxAngle; }

    private float AngularVelocity { get; set; }
    private float NormalizedMaxAngularVelocity { get => normalizedMaxAngularVelocity.Evaluate(AngleRatio); }
    private float MaxAngularVelocity { get => NormalizedMaxAngularVelocity * angularVelocityMultiplier; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        customInteraction = GetComponent<CustomInteraction>();
        customInteraction.OnStartInteraction_Hand += AssignHand;
        customInteraction.OnStartInteraction += CreateAttachPoint;
        customInteraction.OnEndInteraction += DestroyAttachPoint;       

        rb.maxAngularVelocity = MaxAngularVelocity;

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
        rb.maxAngularVelocity = MaxAngularVelocity;

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

            if (continuousHapticFeedback)
            {
                float amp = 
                    processingType == HapticProcessing.Constant ? feedbackAmplitude :
                    processingType == HapticProcessing.GraphBased ? amplitudeGraph.Evaluate(AngleRatio) :
                    processingType == HapticProcessing.OneMinusVelocity ? 1 - NormalizedMaxAngularVelocity : 
                    0f;

                hand.TriggerHapticPulse(1f / 60f, feedbackFrequency, amp > minimumAmplitude ? amp : minimumAmplitude);
            }

            float aV = rotationAxis == MathHelper.Axis.X
                ? rb.angularVelocity.x : rotationAxis == MathHelper.Axis.Y
                ? rb.angularVelocity.y : rb.angularVelocity.z;
            AngularVelocity = inverseAngularVelocity ? -aV : aV;

            LockRotation(AngularVelocity);

            previousAngle = currentAngle;
            currentAngle = Quaternion.Angle(Quaternion.identity, transform.localRotation);
            UpdateHalfRotations();
        }

        //Calculate the actual angle based on half rotations and the current angle.
        ActualAngle = halfRotations * 180 + (backwards ?
            (180 - currentAngle) :
            currentAngle);
    }

    private void LockRotation(float aV)
    {
        //If you're trying to rotate counter-clockwise while the angle is already at 0, 
        //it will keep resetting to prevent you from rotating it any further.
        if (aV < -0.01f && ActualAngle <= 0)
        {
            rb.angularVelocity = Vector3.zero;
            AngularVelocity = 0;
            ActualAngle = 0;
            halfRotations = 0;
            transform.localRotation = initialRotation;
            hand.TriggerHapticPulse(1f / 60f, feedbackFrequency, 1f);
        }

        //Same like previous, but the other way around.
        if (aV > 0.01f && ActualAngle >= MaxAngle)
        {
            rb.angularVelocity = Vector3.zero;
            AngularVelocity = 0;
            ActualAngle = MaxAngle;
            halfRotations = (int)rotationLock * 2;
            transform.localRotation = Quaternion.Euler(0, MaxAngle, 0);
            hand.TriggerHapticPulse(1f / 60f, feedbackFrequency, 1f);
        }
    }

    private void UpdateHalfRotations()
    {
        clockwise = AngularVelocity >= 0;

        if (ActualAngle > MaxAngle)
        {
            ActualAngle = MaxAngle;
            return;
        }
        else if (ActualAngle < 0)
        {
            ActualAngle = 0;
            return;
        }

        if (AngularVelocity != 0 && halfRotationPossible)
        {
            if (clockwise && !backwards && previousAngle > currentAngle)
            {
                halfRotations++;
                backwards = true;
                StartCoroutine(ResetRotationTimer(halfRotationResetTimer));
            }
            else if (clockwise && backwards && currentAngle > previousAngle)
            {
                halfRotations++;
                backwards = false;
                StartCoroutine(ResetRotationTimer(halfRotationResetTimer));
            }
            else if (!clockwise && !backwards && currentAngle > previousAngle)
            {
                halfRotations--;
                backwards = true;
                StartCoroutine(ResetRotationTimer(halfRotationResetTimer));
            }
            else if (!clockwise && backwards && previousAngle > currentAngle)
            {
                halfRotations--;
                backwards = false;
                StartCoroutine(ResetRotationTimer(halfRotationResetTimer));
            }
        }
    }

    private IEnumerator ResetRotationTimer(float time)
    {
        halfRotationPossible = false;

        yield return new WaitForSeconds(time);

        halfRotationPossible = true;
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

        if (previousAttachPoint != null)
        {
            float dist =
                Vector3.Angle(initialAttachPoint.position, transform.position) * Mathf.Rad2Deg -
                Vector3.Angle(previousAttachPoint.position, transform.position) * Mathf.Rad2Deg;

            previousAngle = currentAngle;
            currentAngle += dist;
            if (currentAngle < 0) currentAngle = -currentAngle;
            if (currentAngle > 180) currentAngle = 180 + (180 - currentAngle);
            UpdateHalfRotations();
        }

        if (lockRotationOnInteractionEnd)
            rb.constraints = RigidbodyConstraints.FreezePosition | rotationConstraints;

        isInteracting = true;
    }

    private void DestroyAttachPoint()
    {
        if (initialAttachPoint != null)
        {
            previousAttachPoint = initialAttachPoint;
            Destroy(initialAttachPoint.gameObject);
        }

        if (lockRotationOnInteractionEnd)
            rb.constraints = RigidbodyConstraints.FreezeAll;

        isInteracting = false;
    }
}
