using Valve.VR.InteractionSystem;

public class UserTestThrowable : Throwable
{
    public ObjectDroppingArea LinkedArea { get; set; }

    private ObjectInteractionState interactionState;

    private bool canInteract;

    protected override void Awake()
    {
        base.Awake();

        interactionState = FindObjectOfType<ObjectInteractionState>();
        interactionState.OnUpdateState += UpdateHighlightState;

        UpdateHighlightState(interactionState.CurrentState);
    }

    private void UpdateHighlightState(ObjectInteractionState.State state)
    {
        canInteract = state.Equals(ObjectInteractionState.State.Grabbable);
        interactable.highlightOnHover = canInteract;
    }

    protected override void OnHandHoverBegin(Hand hand)
    {
        if (canInteract)
        {
            base.OnHandHoverBegin(hand);
        }
    }

    protected override void HandHoverUpdate(Hand hand)
    {
        if (canInteract)
        {
            base.HandHoverUpdate(hand);
        }
    }
}
