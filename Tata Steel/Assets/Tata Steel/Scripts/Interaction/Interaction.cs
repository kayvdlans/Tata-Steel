using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Deprecated
{
    public abstract class Interaction : MonoBehaviour
    {
        protected Interactable interactable { get; private set; } = null;
        protected Rigidbody rb { get; private set; } = null;
        protected Transform initialAttachPoint { get; private set; } = null;

        protected bool isInteracting
        {
            get
            {
                if (!interactable)
                    return false;

                return interactable.IsInteracting && interactable.ClosestHand;
            }
        }

        protected void Start()
        {
            //interactable = GetComponent<Interactable>();

            rb = GetComponent<Rigidbody>();

            Initialize();
        }

        protected abstract void Initialize();

        public virtual void OnAttachedToHand(Valve.VR.InteractionSystem.Hand hand)
        {
            //Create an empty object at the position of the hand as soon as you start interacting with the object.
            //Used to calculate the distance traveled and calculate the rotation as well as the offset while grabbing.
            if (initialAttachPoint == null)
            {
                initialAttachPoint = new GameObject(string.Format("[{0}] InitialAttachPoint", this.gameObject.name)).transform;
                initialAttachPoint.position = hand.transform.position;
                initialAttachPoint.rotation = hand.transform.rotation;
                initialAttachPoint.localScale = Vector3.one * 0.25f;
                initialAttachPoint.parent = transform;
            }
        }

        public virtual void OnDetachedFromHand(Valve.VR.InteractionSystem.Hand hand)
        {
            if (initialAttachPoint != null)
                Destroy(initialAttachPoint.gameObject);
        }
    }
}