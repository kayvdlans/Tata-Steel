using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperFunctions
{
    public static float ConfineToBounds(float value, Vector2 bounds)
    {
        return value < bounds.x ? bounds.x : value > bounds.y ? bounds.y : value;
    }
}
