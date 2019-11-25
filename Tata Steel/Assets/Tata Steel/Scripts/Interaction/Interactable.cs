using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    private struct ButtonCombination
    {
        public ButtonCombination(OVRInput.RawButton left, OVRInput.RawButton right)
        {
            Left = left;
            Right = right;
        }

        public OVRInput.RawButton Left { get; private set; }
        public OVRInput.RawButton Right { get; private set; }
    }

    public enum HandMode
    {
        Both,
        LeftOnly,
        RightOnly
    }

    public enum InteractionType
    {
        OnPress,
        OnHold,
        OnRelease
    }

    [Flags]
    public enum SharedInputButton
    {
        None = 0,
        AX = OVRInput.RawButton.A + OVRInput.RawButton.X,
        BY = OVRInput.RawButton.B + OVRInput.RawButton.Y,
        Shoulders = OVRInput.RawButton.LShoulder + OVRInput.RawButton.RShoulder,
        IndexTriggers = OVRInput.RawButton.LIndexTrigger + OVRInput.RawButton.RIndexTrigger,
        HandTriggers = OVRInput.RawButton.LHandTrigger + OVRInput.RawButton.RHandTrigger,
        Any = ~None
    }

    [SerializeField] private SharedInputButton buttonToPress;
    [SerializeField] private HandMode handMode;
    [SerializeField] private InteractionType interactionType;
    [SerializeField] private UnityEvent onStartInteraction;
    [SerializeField] private UnityEvent whileInteracting;
    [SerializeField] private UnityEvent onEndInteraction;
    [SerializeField] private List<Renderer> renderers;
    [SerializeField] private Material selectionMaterial;
    [SerializeField] private bool continueOutOfRange;

    private List<Material[]> originalMaterials = new List<Material[]>();  
    private int materialState = 0; //0 for not interacting, 1 for interacting
    private List<string> buttonNames;
    private OVRInput.RawButton lastInput = OVRInput.RawButton.None;

    public bool CanInteract { get; set; } = false;

    public bool IsInteracting { get; private set; }

    public Hand ClosestHand { get; set; } = null;

    public UnityEvent OnStartInteraction { get { return onStartInteraction; } }

    public UnityEvent OnEndInteraction { get { return onEndInteraction; } }

    public OVRInput.Controller Controller { get; set; } = OVRInput.Controller.None;

    private Dictionary<SharedInputButton, ButtonCombination> sharedInputLinks = new Dictionary<SharedInputButton, ButtonCombination>()
    {
        { SharedInputButton.AX, new ButtonCombination(OVRInput.RawButton.A, OVRInput.RawButton.X) },
        { SharedInputButton.BY, new ButtonCombination(OVRInput.RawButton.B, OVRInput.RawButton.Y) },
        { SharedInputButton.HandTriggers, new ButtonCombination(OVRInput.RawButton.LHandTrigger, OVRInput.RawButton.RHandTrigger) },
        { SharedInputButton.IndexTriggers, new ButtonCombination(OVRInput.RawButton.LIndexTrigger, OVRInput.RawButton.RIndexTrigger) },
        { SharedInputButton.Shoulders, new ButtonCombination(OVRInput.RawButton.LShoulder, OVRInput.RawButton.RShoulder) }
    };

    private void Start()
    {
        for (int i = 0; i < renderers.Count; i++)
            originalMaterials.Add(renderers[i].materials);
    }

    private bool InputValid()
    {
        if (buttonNames == null)
        {
            buttonNames = new List<string>();
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

                    buttonNames.Add(s);

                    lastIndex = i + 2;
                }
            }

            string last = "";
            for (int i = lastIndex; i < b.Length; i++)
            {
                last += b[i];
            }   
            buttonNames.Add(last);
        }

        if (Controller == OVRInput.Controller.LTouch && handMode != HandMode.RightOnly)
            return CheckForInputFromButtonNames(buttonNames, true);
        else if (Controller == OVRInput.Controller.RTouch && handMode != HandMode.LeftOnly)
            return CheckForInputFromButtonNames(buttonNames, false);

        return false;   
    }

    private bool CheckForInputFromButtonNames(List<string> buttons, bool left)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            ButtonCombination input = sharedInputLinks[GetButtonByName(buttons[i])];
            OVRInput.RawButton button = left ? input.Left : input.Right;

            switch (interactionType)
            {
                case InteractionType.OnHold:
                    if (OVRInput.Get(button))
                    {
                        lastInput = button;
                        return true;
                    }
                    break;
                case InteractionType.OnPress:
                    if (OVRInput.GetDown(button))
                    {
                        lastInput = button;
                        return true;
                    }
                    break;
                case InteractionType.OnRelease:
                    if (OVRInput.GetUp(button))
                    {
                        lastInput = button;
                        return true;
                    }
                    break;
            }
        }

        return false;
    }


    private SharedInputButton GetButtonByName(string name)
    {
        Array t = Enum.GetValues(typeof(SharedInputButton));

        for (int i = 0; i < t.Length; i++)
        {
            if (t.GetValue(i).ToString().Equals(name))
            {
                return (SharedInputButton) t.GetValue(i);
            }
        }

        return SharedInputButton.None;
    }

    private void Update()
    {
        //Update renderers.
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
                    renderers[i].material = selectionMaterial;

                materialState = 1;
            }
        }

        //Fire interaction events
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

                    if (ClosestHand)
                        ClosestHand.IsInteracting = true;
                }
                else if (IsInteracting)
                {
                    IsInteracting = false;

                    if (ClosestHand)
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
                 
                if (!continueOutOfRange || lastInput != OVRInput.RawButton.None && !OVRInput.Get(lastInput))
                {
                    IsInteracting = false;

                    if (ClosestHand)
                        ClosestHand.IsInteracting = false;
                    onEndInteraction.Invoke();
                }
            }
        }
    }
}
