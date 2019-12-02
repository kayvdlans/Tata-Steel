using UnityEngine;

namespace Deprecated
{
    public class InteractionRotate : Interaction
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

        [SerializeField] private bool inverseAngularVelocity = true;

        //Works as a replacement for mass since the force mode of our rotation is VelocityChange,
        //which does not get affected by mass.
        private const float DELTA_MAGIC = 1f;

        private float currentAngle = 0;
        private float previousAngle = 0;
        [SerializeField] private int halfRotations = 0;
        private bool clockwise = true;
        private bool backwards = false;

        private RigidbodyConstraints rotationConstraints;
        private Quaternion initialRotation;

        public float ActualAngle { get; private set; } = 0;

        protected override void Initialize()
        {
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
                //Checks the change in position from the initial point the player started interacting 
                //with the object and the current position of the hand.
                Vector3 positionDelta = (interactable.ClosestHand.transform.position - initialAttachPoint.position) * DELTA_MAGIC;
                //Add force based on the change of position.
                rb.AddForceAtPosition(positionDelta, initialAttachPoint.position, ForceMode.VelocityChange);
            }

            float aV = rotationAxis == MathHelper.Axis.X
                ? rb.angularVelocity.x : rotationAxis == MathHelper.Axis.Y
                ? rb.angularVelocity.y : rb.angularVelocity.z;
            float angularVelocity = inverseAngularVelocity ? -aV : aV;
            LockRotation(angularVelocity);


            currentAngle = Quaternion.Angle(initialRotation, transform.localRotation);
            Debug.Log(currentAngle);

            clockwise = angularVelocity >= 0;

            //If there is movement, check whether a half rotation should be added.
            //This is to make sure the actual rotation of the object is being documented,
            //Since normally the current angle only fluctuates between 0 and 180.
            if (Mathf.Abs(aV) > 0.01f)
            {
                UpdateHalfRotations();
            }

            //Debug.Log()

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

        public override void OnInteractionStart()
        {
            base.OnInteractionStart();

            if (lockRotationOnInteractionEnd)
                rb.constraints = RigidbodyConstraints.FreezePosition | rotationConstraints;
        }

        public override void OnInteractionEnd()
        {
            base.OnInteractionEnd();

            if (lockRotationOnInteractionEnd)
                rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}