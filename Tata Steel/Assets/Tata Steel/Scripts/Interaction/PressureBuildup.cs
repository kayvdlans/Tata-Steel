using UnityEngine;

public class PressureBuildup : MonoBehaviour
{
    [SerializeField] private InteractionRotate valve;
    [SerializeField] private Openable gate;
    [SerializeField] private PressureMeter meter;

    [SerializeField] public Vector2 bounds;
    [SerializeField] public float currentValue;

    private Interactable valveInteractable;    

    public float CurrentValue { get => currentValue; }

    private void Start()
    {
        valveInteractable = valve.GetComponent<Interactable>();
    }

    private void FixedUpdate()
    {
        if (valveInteractable.isInteracting)
        {
            gate.UpdateValue(valve.ActualAngle, bounds, ref currentValue);
            meter.UpdateAngle(MathHelper.NormalizeValueBetweenBounds(currentValue, bounds));
        }
    }
}
