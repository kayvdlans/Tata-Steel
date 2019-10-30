using System;
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

    //Dont use mixed, since it doesnt work well in this scenario, just use different instances
    [SerializeField] private OVRInput.RawButton buttonToPress;
    [SerializeField] private InteractionType interactionType;
    [SerializeField] private UnityEvent onStartInteraction;
    [SerializeField] private UnityEvent whileInteracting;
    [SerializeField] private UnityEvent onEndInteraction;
    [SerializeField] private bool continueOutOfRange;

    //[SerializeField] private Material material;
    [SerializeField] private Material outlineMaterial;

    [SerializeField] private List<Renderer> renderers;

    private List<Material[]> originalMaterials = new List<Material[]>();
    private bool canInteract = false;

    public bool isInteracting { get; private set; }

    public List<Transform> hands { get; private set; } = new List<Transform>();

    public Transform closestHand { get; private set; } = null;

    public OVRInput.Controller controller { get; private set; } = OVRInput.Controller.None;

    private void Start()
    {
        if (renderers.Count != 0)
        {
            foreach (Renderer r in renderers)
            {
                originalMaterials.Add(r.materials);
            }
        }

        StartCoroutine(CheckForClosestHand(0.1f));
    }

    private bool InputValid()
    {
        List<string> buttons = new List<string>();
        char[] b = buttonToPress.ToString().ToCharArray();
        int lastIndex = 0;

        for (int i = 0; i < b.Length; i++)
        {
            if (b[i] == ',')
            {
                string s = "";
                for (int j = lastIndex; j < i; j++)
                {
                    s += b[j];
                }

                buttons.Add(s);

                lastIndex = i + 2;
            }
        }

        string last = "";
        for (int i = lastIndex; i < b.Length; i++)
        {
            last += b[i];
        }
        buttons.Add(last);

        List<string> leftButtons = new List<string>();
        List<string> rightButtons = new List<string>();

        //Since the RawButtons are divided into LIndexTrigger, RIndexTrigger, etc to show what controller is currently being used, 
        //we can use the prefix to get the side at which it is being pressed. 
        //This way we can limit the input to the controller which is closest to the object. 
        for (int i = 0; i < buttons.Count; i++)
            if (buttons[i].Substring(0, 1) == "L")
                leftButtons.Add(buttons[i]);
            else if (buttons[i] == "X" || buttons[i] == "Y")
                leftButtons.Add(buttons[i]);
            else if (buttons[i].Substring(0, 1) == "R")
                rightButtons.Add(buttons[i]);            
            else if (buttons[i] == "A" || buttons[i] == "B")
                rightButtons.Add(buttons[i]);

        if (controller == OVRInput.Controller.LTouch)
            return CheckForInputFromButtonNames(leftButtons);
        else if (controller == OVRInput.Controller.RTouch)
            return CheckForInputFromButtonNames(rightButtons);

        return false;   
    }

    private bool CheckForInputFromButtonNames(List<string> buttons)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            switch (interactionType)
            {
                case InteractionType.OnHold:
                    if (OVRInput.Get(GetButtonByName(buttons[i])))
                        return true;
                    break;
                case InteractionType.OnPress:
                    if (OVRInput.GetDown(GetButtonByName(buttons[i])))
                        return true;
                    break;
                case InteractionType.OnRelease:
                    if (OVRInput.GetUp(GetButtonByName(buttons[i])))
                        return true;
                    break;
            }
        }

        return false;
    }

    private OVRInput.RawButton GetButtonByName(string name)
    {
        Array t = Enum.GetValues(typeof(OVRInput.RawButton));

        for (int i = 0; i < t.Length; i++)
        {
            if (t.GetValue(i).ToString().Equals(name))
            {
                return (OVRInput.RawButton) t.GetValue(i);
            }
        }

        return OVRInput.RawButton.None;
    }

    private void FixedUpdate()
    {
        if (canInteract)
        {
            if (InputValid())
            {
                if (!isInteracting)
                {
                    onStartInteraction.Invoke();
                }

                //Make sure to not call while interacting in the first frame of interaction.
                if (isInteracting && interactionType == InteractionType.OnHold)
                {
                    whileInteracting.Invoke();
                }

                isInteracting = true;
            }
            else if (isInteracting)
            {
                isInteracting = false;
                onEndInteraction.Invoke();
            }
        }
        else if (isInteracting)
        {
            if (interactionType == InteractionType.OnHold)
            {
                whileInteracting.Invoke();
            }

            if (!continueOutOfRange || !OVRInput.Get(buttonToPress))
            {
                isInteracting = false;
                onEndInteraction.Invoke();
            }
        }
    }

    //this will have to be replaced to a hand(s) script keeping track of objects instead of the other way around.
    //if there are a lot of objects this will hit the performance quite a bit the way it is right now,
    //even though it only gets called 10 times a second.
    //This also does not account for different items being in reach of the hand which might become a problem in the future.
    //We'll have to see though. I'm too depressed to change this right now.
    private IEnumerator CheckForClosestHand(float time)
    {
        while (true)
        {
            if (hands == null || hands.Count == 0 || (!isInteracting && !canInteract))
            {
                closestHand = null;
            }

            if (hands != null && hands.Count > 0 && !isInteracting)
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

                controller = closestHand.name == "LeftHand" ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;
            }
            else
            {
                for (int i = 0; i < renderers.Count; i++)
                    renderers[i].materials = originalMaterials[i];
                canInteract = false;

                controller = OVRInput.Controller.None;
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
