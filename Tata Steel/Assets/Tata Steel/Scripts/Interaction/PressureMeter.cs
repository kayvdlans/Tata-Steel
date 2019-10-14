using UnityEngine;

public class PressureMeter : MonoBehaviour
{
    [SerializeField] private Transform meter;
    [SerializeField] private Vector2 bounds;
    [SerializeField] private Vector2 offsetBounds;
    [SerializeField] private Vector2 offsetTickBounds;

    private float angle; //angle needs to be calculated based on the bounds and the actual pressure in the pipes. so how do we do that, you wonder? well I dont know yet, so kill me. :)
    private float offset;
    private float offsetMultiplier;
    private float combinedAngle;

    private void Start()
    {
        if (bounds.x > bounds.y)
            Debug.LogWarning("You're an idiot.");

        angle = bounds.x;
    }

    private void FixedUpdate()
    {
        //just do the random offset part here instead of everything, handle angle update from pressurebuildup script

        float multiplier = 1f; //update multiplier at some point
        offset += Random.Range(offsetTickBounds.x * multiplier, offsetTickBounds.y * multiplier);
        offset = MathHelper.ConfineToBounds(offset, offsetBounds);

        combinedAngle = (angle == bounds.x) ? angle : angle + (offset * multiplier);
        combinedAngle = MathHelper.ConfineToBounds(combinedAngle, bounds);

        meter.localRotation = Quaternion.Euler(0, -90, combinedAngle);
    }

    public void UpdateAngle(float normalizedValue)
    {
        angle = MathHelper.GetValueBetweenBoundsFromNormalizedValue(normalizedValue, bounds);
    }
}
