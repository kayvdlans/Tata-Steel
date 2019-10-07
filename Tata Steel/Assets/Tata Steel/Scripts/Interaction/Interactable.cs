using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public enum InteractionType
    {
        OnPress,
        OnHold,
        OnRelease
    }

    [SerializeField] private OVRInput.Button buttonToPress;
    [SerializeField] private InteractionType interactionType;
    [SerializeField] private UnityEvent onStartInteraction;
    [SerializeField] private UnityEvent onEndInteraction;

    [SerializeField] private Material material;
    [SerializeField] private Material outlineMaterial;

    [SerializeField] private List<Renderer> renderers;

    private bool canInteract = false;

    public bool isInteracting { get; private set; }

    public Transform hand { get; private set; } = null;

    private void Update()
    {
        if (canInteract)
        {
            switch (interactionType)
            {
                case InteractionType.OnHold:
                    if (OVRInput.Get(buttonToPress))
                    {
                        if (!isInteracting)
                        {
                            onStartInteraction.Invoke();
                        }
                        isInteracting = true;
                    }
                    else if (isInteracting)
                    {
                        isInteracting = false;
                        onEndInteraction.Invoke();
                    }
                    break;
                case InteractionType.OnPress:
                    if (OVRInput.GetDown(buttonToPress))
                    {
                        onStartInteraction.Invoke();
                    }
                    break;
                case InteractionType.OnRelease:
                    if (OVRInput.GetUp(buttonToPress))
                    {
                        onStartInteraction.Invoke();
                    }
                    break;
            }
        }
        else if (isInteracting)
        {
            if (!OVRInput.Get(buttonToPress))
            {
                isInteracting = false;
                onEndInteraction.Invoke();
            }
        }
        else
        {
            hand = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerController"))
        {
            if (!hand)
                hand = other.transform;

            foreach (Renderer r in renderers)
                r.material = outlineMaterial;
            canInteract = true;
        }
    }  

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerController"))
        {
            foreach (Renderer r in renderers)
                r.material = material;
            canInteract = false;
        }
    }
}
