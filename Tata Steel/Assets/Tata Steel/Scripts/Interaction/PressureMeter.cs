using UnityEngine;

public class PressureMeter : MonoBehaviour
{
    [SerializeField] private RotateValve valve;
    [SerializeField] private Vector2 angleMinMax;
    [SerializeField] private Vector2 offsetMinMax;
    [SerializeField] private Vector2 offsetPerTickMinMax;

    private float currentAngle;
    private float currentOffset;
    private float combinedAngle;

    private void Start()
    {
        if (angleMinMax.x > angleMinMax.y 
            || offsetMinMax.x > offsetMinMax.y 
            || offsetPerTickMinMax.x > offsetPerTickMinMax.y)
            Debug.LogWarning("You're an idiot.");

        currentAngle = angleMinMax.x;
    }

    private void FixedUpdate()
    {
        currentAngle = MathHelper.GetValueBetweenBoundsFromNormalizedValue(valve.AngleRatio, angleMinMax);

        currentOffset += Random.Range(offsetPerTickMinMax.x, offsetPerTickMinMax.y);
        currentOffset = MathHelper.ConfineToBounds(currentOffset, offsetMinMax);

        combinedAngle = currentAngle == angleMinMax.x ? currentAngle : currentAngle + currentOffset;
        combinedAngle = MathHelper.ConfineToBounds(combinedAngle, angleMinMax);

        transform.localRotation = Quaternion.Euler(0, -90, combinedAngle);
    }

    //Deprecated
    public void UpdateAngle(float normalizedValue)
    {
        currentAngle = MathHelper.GetValueBetweenBoundsFromNormalizedValue(normalizedValue, angleMinMax);
    }
}
