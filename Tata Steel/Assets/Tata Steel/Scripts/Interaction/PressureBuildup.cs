using UnityEngine;

public class PressureBuildup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InteractionInstantRotation valve;
    [SerializeField] private PressureMeter meter;

    [Header("Angle Variables")]
    [SerializeField] private float angleToOpen;
    [SerializeField] private float angleToLock;

    [Space]
    [SerializeField] private Vector2 bounds;
    private float currentValue;  

    public float CurrentValue { get => currentValue; }
    public float AngleToLock { get => angleToLock; }

    private void Awake()
    {
        valve.Buildup = this;
    }

    private void Update()
{
        float v = MathHelper.ConfineToBounds(valve.CurrentAngle / angleToOpen, new Vector2(0, 1));
        currentValue = MathHelper.GetValueBetweenBoundsFromNormalizedValue(v, bounds);
        meter.UpdateAngle(MathHelper.NormalizeValueBetweenBounds(currentValue, bounds));
    }
}
