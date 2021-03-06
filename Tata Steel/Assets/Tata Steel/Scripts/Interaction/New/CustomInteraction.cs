﻿using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class CustomInteraction : MonoBehaviour
{
    public enum InteractionType
    {
        OnPress,
        OnHold
    }

    public bool restoreOriginalParent = false;

    public Hand.AttachmentFlags attachmentFlags;
    public GrabTypes action;
    public InteractionType interactionType;

    public UnityAction OnTouch;
    public bool fireTouchEventContinously;
    [Space]
    public UnityAction OnStartInteraction;
    public UnityAction OnEndInteraction;
    public UnityAction WhileInteracting;

    public UnityAction<Hand> OnStartInteraction_Hand;

    [HideInInspector]
    public Interactable interactable;

    protected bool attached = false;
    protected float attachTime;
    protected Vector3 attachPosition;
    protected Quaternion attachRotation;

    protected virtual void Awake()
    {
        interactable = GetComponent<Interactable>();
    }


    //-------------------------------------------------
    protected virtual void OnHandHoverBegin(Hand hand)
    {
        bool showHint = false;
        GrabTypes bestGrabType = hand.GetGrabStarting(action);

        if (bestGrabType != GrabTypes.None)
        {
            hand.AttachObject(gameObject, bestGrabType, attachmentFlags);
            Debug.LogError("wow");
            OnStartInteraction?.Invoke();
        }

        if (showHint)
        {
            hand.ShowGrabHint();
        }

        OnTouch?.Invoke();
    }


    //-------------------------------------------------
    protected virtual void OnHandHoverEnd(Hand hand)
    {
        hand.HideGrabHint();
    }


    //-------------------------------------------------
    protected virtual void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting(action);

        if (startingGrabType != GrabTypes.None)
        {
            hand.AttachObject(gameObject, startingGrabType, attachmentFlags);

            OnStartInteraction_Hand?.Invoke(hand);
            OnStartInteraction?.Invoke();
        }

        if (startingGrabType != GrabTypes.None)
        {
            hand.HideGrabHint();
        }

        if (fireTouchEventContinously)
            OnTouch?.Invoke();
    }

    //-------------------------------------------------
    protected virtual void OnAttachedToHand(Hand hand)
    {
        attached = true;

        hand.HoverLock(null);

        attachTime = Time.time;
        attachPosition = transform.position;
        attachRotation = transform.rotation;
    }


    //-------------------------------------------------
    protected virtual void OnDetachedFromHand(Hand hand)
    {
        attached = false;

        OnEndInteraction?.Invoke();

        hand.HoverUnlock(null);
    }

    //-------------------------------------------------
    protected virtual void HandAttachedUpdate(Hand hand)
    {
        if (hand.IsGrabEnding(this.gameObject))
        {
            hand.DetachObject(gameObject, restoreOriginalParent);
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
        }

        if (interactionType == InteractionType.OnHold)
        {
            WhileInteracting?.Invoke();
        }
    }


    //-------------------------------------------------
    protected virtual IEnumerator LateDetach(Hand hand)
    {
        yield return new WaitForEndOfFrame();

        hand.DetachObject(gameObject, restoreOriginalParent);
    }


    //-------------------------------------------------
    protected virtual void OnHandFocusAcquired(Hand hand)
    {
        gameObject.SetActive(true);
    }


    //-------------------------------------------------
    protected virtual void OnHandFocusLost(Hand hand)
    {
        gameObject.SetActive(false);
    }
}
