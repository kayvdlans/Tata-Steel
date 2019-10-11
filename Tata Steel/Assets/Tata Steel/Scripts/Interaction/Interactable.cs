using System.Collections;
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

    public List<Transform> hands { get; private set; } = new List<Transform>();

    public Transform closestHand { get; private set; } = null;

    private void Start()
    {
        StartCoroutine(CheckForClosestHand(0.1f));
    }

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
    }

    private IEnumerator CheckForClosestHand(float time)
    {
        while (true)
        {
            if (hands == null || hands.Count == 0 || (!isInteracting && !canInteract))
            {
                closestHand = null;
            }

            if (hands != null && hands.Count > 0)
            {
                float closestDistance = float.MaxValue;

                foreach (Transform hand in hands)
                {
                    float distance = Vector3.Distance(transform.position, hand.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestHand = hand;
                    }
                }
            }

            if (closestHand != null)
            {
                foreach (Renderer r in renderers)
                    r.material = outlineMaterial;
                canInteract = true;
            }
            else
            {
                foreach (Renderer r in renderers)
                    r.material = material;
                canInteract = false;
            }

            yield return new WaitForSeconds(time);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerController"))
        {
            if (!hands.Contains(other.transform))
                hands.Add(other.transform);

            StopAllCoroutines();
            StartCoroutine(CheckForClosestHand(0.1f));
        }
    }  

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerController"))
        {
            if (hands.Contains(other.transform))
                hands.Remove(other.transform);

            StopAllCoroutines();
            StartCoroutine(CheckForClosestHand(0.1f));
        }
    }
}
