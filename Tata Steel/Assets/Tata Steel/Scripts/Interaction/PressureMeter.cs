using System.Collections;
using System.Collections.Generic;
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
    }

    private void FixedUpdate()
    {
        //just do the random offset part here instead of everything, handle angle update from pressurebuildup script

        float multiplier = 1f;
        offset += Random.Range(offsetTickBounds.x * multiplier, offsetTickBounds.y * multiplier);
        offset = HelperFunctions.ConfineToBounds(offset, offsetBounds);

        combinedAngle = (angle == bounds.x) ? angle : angle + (offset * multiplier);
        combinedAngle = HelperFunctions.ConfineToBounds(combinedAngle, bounds);

        meter.rotation = Quaternion.Euler(combinedAngle, 0, 0);
    }



    public void UpdateAngle(float normalizedValue)
    {

    }
}
