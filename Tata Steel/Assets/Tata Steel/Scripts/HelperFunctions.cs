using UnityEngine;

public static class MathHelper
{
    public static float ConfineToBounds(float value, Vector2 bounds)
    {
        return value < bounds.x ? bounds.x : value > bounds.y ? bounds.y : value;
    }

    public static float NormalizeValueBetweenBounds(float value, Vector2 bounds)
    {
        return  (value - bounds.x) / (bounds.y - bounds.x);
    }

    public static float GetValueBetweenBoundsFromNormalizedValue(float normalizedValue, Vector2 bounds)
    {
        return bounds.x + (bounds.y - bounds.x) * normalizedValue;
    }

    public enum Axis
    {
        X = 0,
        Y = 1,
        Z = 2
    }
}
