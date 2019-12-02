using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Valve.VR.InteractionSystem.Interactable))]
public class CustomInteraction : MonoBehaviour
{
    public enum InteractionType
    {
        OnPress,
        OnHold
    }

    public bool restoreOriginalParent = false;

    public Valve.VR.InteractionSystem.Hand.AttachmentFlags attachmentFlags;
    public GrabTypes action;
    public InteractionType interactionType;

    [Header("Touch Events")]
    public UnityEvent onTouch;
    public bool fireTouchEventContinously;
    [Space]
    public UnityEvent onStartInteraction;
    public UnityEvent onEndInteraction;
    public UnityEvent whileInteracting;

    [HideInInspector]
    public Interactable interactable;

    protected bool attached = false;
    protected float attachTime;
    protected Vector3 attachPosition;
    protected Quaternion attachRotation;

    private bool firstInput = true;

    protected virtual void Awake()
    {
        interactable = GetComponent<Interactable>();
    }


    //-------------------------------------------------
    protected virtual void OnHandHoverBegin(Valve.VR.InteractionSystem.Hand hand)
    {
        bool showHint = false;
        GrabTypes bestGrabType = hand.GetGrabStarting(action);

        if (bestGrabType != GrabTypes.None)
        {
            hand.AttachObject(gameObject, bestGrabType, attachmentFlags);
            onStartInteraction.Invoke();
        }

        if (showHint)
        {
            hand.ShowGrabHint();
        }

        onTouch.Invoke();
    }


    //-------------------------------------------------
    protected virtual void OnHandHoverEnd(Valve.VR.InteractionSystem.Hand hand)
    {
        hand.HideGrabHint();
    }


    //-------------------------------------------------
    protected virtual void HandHoverUpdate(Valve.VR.InteractionSystem.Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting(action);

        if (startingGrabType != GrabTypes.None)
        {
            hand.AttachObject(gameObject, startingGrabType, attachmentFlags);
            onStartInteraction.Invoke();
        }

        if (startingGrabType != GrabTypes.None)
        {
            hand.HideGrabHint();
        }

        if (fireTouchEventContinously)
            onTouch.Invoke();
    }

    //-------------------------------------------------
    protected virtual void OnAttachedToHand(Valve.VR.InteractionSystem.Hand hand)
    {
        attached = true;

        hand.HoverLock(null);

        attachTime = Time.time;
        attachPosition = transform.position;
        attachRotation = transform.rotation;
    }


    //-------------------------------------------------
    protected virtual void OnDetachedFromHand(Valve.VR.InteractionSystem.Hand hand)
    {
        attached = false;

        hand.HoverUnlock(null);

        firstInput = true;
    }

    //-------------------------------------------------
    protected virtual void HandAttachedUpdate(Valve.VR.InteractionSystem.Hand hand)
    {
        if (hand.IsGrabEnding(this.gameObject))
        {
            hand.DetachObject(gameObject, restoreOriginalParent);
            onEndInteraction.Invoke();
            // Uncomment to detach ourselves late in the frame.
            // This is so that any vehicles the player is attached to
            // have a chance to finish updating themselves.
            // If we detach now, our position could be behind what it
            // will be at the end of the frame, and the object may appear
            // to teleport behind the hand when the player releases it.
            //StartCoroutine( LateDetach( hand ) );
        }

        if (interactionType == InteractionType.OnPress)
        {
            hand.DetachObject(gameObject, restoreOriginalParent);
            onEndInteraction.Invoke();
        }

        if (interactionType == InteractionType.OnHold)
        {
            whileInteracting.Invoke();
        }
    }


    //-------------------------------------------------
    protected virtual IEnumerator LateDetach(Valve.VR.InteractionSystem.Hand hand)
    {
        yield return new WaitForEndOfFrame();

        hand.DetachObject(gameObject, restoreOriginalParent);
    }


    //-------------------------------------------------
    protected virtual void OnHandFocusAcquired(Valve.VR.InteractionSystem.Hand hand)
    {
        gameObject.SetActive(true);
    }


    //-------------------------------------------------
    protected virtual void OnHandFocusLost(Valve.VR.InteractionSystem.Hand hand)
    {
        gameObject.SetActive(false);
    }
}
