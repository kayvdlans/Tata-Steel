using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
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
    [SerializeField] private bool continueOutOfRange;

    private Material[] originalMaterials;
    private Renderer r;
    private int materialState = 0; //0 for not interacting, 1 for interacting

    public bool CanInteract { get; set; } = false;

    public bool IsInteracting { get; private set; }

    public Hand ClosestHand { get; set; } = null;

    public OVRInput.Controller Controller { get; set; } = OVRInput.Controller.None;

    private void Start()
    {
        MeshFilter[] children = transform.GetComponentsInChildren<MeshFilter>();
        children = children.Where(val => val.CompareTag("SelectionMaterial")).ToArray();
        CombineInstance[] combine = new CombineInstance[children.Length];

        for (int i = 0; i < combine.Length; i++)
        {
            combine[i].mesh = children[i].sharedMesh;
            combine[i].transform = children[i].transform.localToWorldMatrix;
            children[i].gameObject.SetActive(false);
        }

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);
        GetComponent<MeshFilter>().mesh = mesh;

        r = GetComponent<Renderer>();
        //originalMaterials = new Material[r.sharedMaterials.Length];
        originalMaterials = r.sharedMaterials;
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
            r.sharedMaterials = originalMaterials;
            materialState = 0;
        }
        else if (CanInteract && materialState != 1)
        {
            for (int i = 0; i < r.sharedMaterials.Length; i++)
                r.sharedMaterials[i] = ClosestHand.SelectionMaterial;
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
