using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Openable : MonoBehaviour
{
    [SerializeField] private MathHelper.Axis axis;
    [SerializeField] private Vector2 valueBounds;
    [SerializeField] private Vector2 physicalBounds;

    public void UpdateValue(float angle, ref float value)
    {
        //take angle to calculate how far its opened
        //normalized physical value. 
        //
        float normalizedPhysicalValue = 0.5f;
        value = MathHelper.GetValueBetweenBoundsFromNormalizedValue(normalizedPhysicalValue, valueBounds);
    }
}
