using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureBuildup : MonoBehaviour
{
    [SerializeField] private InteractionRotate valve;
    [SerializeField] private PressureMeter meter;

    [SerializeField] private Vector2 bounds;
    [SerializeField] private float currentValue;

    private Interactable valveInteractable;

    private void FixedUpdate()
    {
        if (valveInteractable.isInteracting)
        {
            meter.UpdateAngle((currentValue - bounds.x) / (bounds.y - bounds.x));
        }
    }
}
