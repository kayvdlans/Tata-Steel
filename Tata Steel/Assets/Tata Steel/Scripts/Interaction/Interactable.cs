using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public enum InputButtons
    {
        None = 0,
        AX = OVRInput.RawButton.A | OVRInput.RawButton.X,
        BY = OVRInput.RawButton.B | OVRInput.RawButton.Y,
        Shoulders = OVRInput.RawButton.LShoulder | OVRInput.RawButton.RShoulder,
        IndexTriggers = OVRInput.RawButton.LIndexTrigger | OVRInput.RawButton.RIndexTrigger,
        HandTriggers = OVRInput.RawButton.LHandTrigger | OVRInput.RawButton.RHandTrigger,
        Thumbsticks = OVRInput.RawButton.LThumbstick | OVRInput.RawButton.RThumbstick,
        Any = ~None
    }

    //Dont use mixed, since it doesnt work well in this scenario, just use different instances
    [SerializeField] private InputButtons buttonToPress;
    [SerializeField] private InteractionType interactionType;
    [SerializeField] private UnityEvent onStartInteraction;
    [SerializeField] private UnityEvent whileInteracting;
    [SerializeField] private UnityEvent onEndInteraction;
    [SerializeField] private List<Renderer> renderers;
    [SerializeField] private bool continueOutOfRange;

    private List<Material[]> originalMaterials = new List<Material[]>();  
    private int materialState = 0; //0 for not interacting, 1 for interacting

    public bool CanInteract { get; set; } = false;

    public bool IsInteracting { get; private set; }

    public Hand ClosestHand { get; set; } = null;

    public OVRInput.Controller Controller { get; set; } = OVRInput.Controller.None;

    private void Start()
    {
        for (int i = 0; i < renderers.Count; i++)
            originalMaterials.Add(renderers[i].materials);
        //originalMaterials = r.sharedMaterials;
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

        if (Controller == OVRInput.Controller.LTouch)
            return CheckForInputFromButtonNames(leftButtons);
        else if (Controller == OVRInput.Controller.RTouch)
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
                    if (OVRInput.Get((OVRInput.RawButton)GetButtonByName(buttons[i])))
                        return true;
                    break;
                case InteractionType.OnPress:
                    if (OVRInput.GetDown((OVRInput.RawButton)GetButtonByName(buttons[i])))
                        return true;
                    break;
                case InteractionType.OnRelease:
                    if (OVRInput.GetUp((OVRInput.RawButton)GetButtonByName(buttons[i])))
                        return true;
                    break;
            }
        }

        return false;
    }

    private InputButtons GetButtonByName(string name)
    {
        Array t = Enum.GetValues(typeof(InputButtons));

        for (int i = 0; i < t.Length; i++)
        {
            if (t.GetValue(i).ToString().Equals(name))
            {
                return (InputButtons) t.GetValue(i);
            }
        }

        return InputButtons.None;
    }

    private void Update()
    {
        if (!CanInteract && materialState != 0)
        {
            for (int i = 0; i < renderers.Count; i++)
                renderers[i].materials = originalMaterials[i];

            materialState = 0;
        }
        else if (CanInteract && materialState != 1)
        {
            for (int i = 0; i < renderers.Count; i++)
                for (int j = 0; j < renderers[i].materials.Length; j++)
                    renderers[i].materials[j] = ClosestHand.SelectionMaterial;

            materialState = 1;
        }
    }

    private void FixedUpdate()
    {
        if (CanInteract)
        {
            if (InputValid())
            {
                if (!IsInteracting)
                {
                    onStartInteraction.Invoke();
                }

                //Make sure to not call while interacting in the first frame of interaction.
                if (IsInteracting && interactionType == InteractionType.OnHold)
                {
                    whileInteracting.Invoke();
                }

                IsInteracting = true;
                ClosestHand.IsInteracting = true;
            }
            else if (IsInteracting)
            {
                IsInteracting = false;
                ClosestHand.IsInteracting = false;
                onEndInteraction.Invoke();
            }
        }
        else if (IsInteracting)
        {
            if (interactionType == InteractionType.OnHold)
            {
                whileInteracting.Invoke();
            }

            if (!continueOutOfRange || !OVRInput.Get((OVRInput.RawButton)buttonToPress))
            {
                IsInteracting = false;
                ClosestHand.IsInteracting = false;
                onEndInteraction.Invoke();
            }
        }
    }
}
