using UnityEngine;

public class Openable : MonoBehaviour
{
    [SerializeField] private MathHelper.Axis axis;
    [SerializeField] private Vector2 physicalBounds;
    [SerializeField] private float angleToFullyOpen;

    [Tooltip("Effectivity defines how much effect the valve has on the opening of this object.")]
    [SerializeField] private float effectivity = 1f;

    [Tooltip("The mass of this object slows down the speed at which the object opens.")]
    [SerializeField] private float mass = 1f;

    private Vector3 basePosition;

    private void Start()
    {
        basePosition = transform.position;
    }

    public void UpdateValue(float angle, Vector2 valueBounds, ref float value)
    {
        float v = angle * effectivity / mass;
        v /= angleToFullyOpen;

        float physicalValue = MathHelper.GetValueBetweenBoundsFromNormalizedValue(v, physicalBounds);

        switch (axis)
        {
            case MathHelper.Axis.X:
                transform.position = new Vector3(physicalValue, basePosition.y, basePosition.z);
                break;
            case MathHelper.Axis.Y:
                transform.position = new Vector3(basePosition.x, physicalValue, basePosition.z);
                break;
            case MathHelper.Axis.Z:
                transform.position = new Vector3(basePosition.x, basePosition.y, physicalValue);
                break;
        }

        value = MathHelper.GetValueBetweenBoundsFromNormalizedValue(v, valueBounds);
    }
}
