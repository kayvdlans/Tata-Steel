﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Currently just use Flip() to turn the machine on or off using the Interactable script
/// </summary>
[RequireComponent(typeof(Interactable), typeof(BoxCollider))]
public class FlipSwitch : MonoBehaviour
{
    [SerializeField] private Transform s;
    [SerializeField] private MathHelper.Axis axis;
    [SerializeField] private Vector2 offOnRotation;
    [SerializeField] private bool startOn;

    private Vector3 originalEulers = Vector3.zero;
    private float currentAngle = 0;

    public GameObject pc;
    public bool CurrentlyTurnedOn { get; private set; } = false;

    private void Start()
    {
        originalEulers = s.localEulerAngles;
        UpdateValue(startOn); 
    }

    public void Flip()
    {
        UpdateValue(!CurrentlyTurnedOn);
        pc.SetActive(!pc.activeSelf);
    }

    private void UpdateValue(bool on)
    {
        CurrentlyTurnedOn = on;

        currentAngle = on ? offOnRotation.y : offOnRotation.x;

        s.localEulerAngles = new Vector3(
            (int)axis == 0 ? currentAngle : originalEulers.x,
            (int)axis == 1 ? currentAngle : originalEulers.y,
            (int)axis == 2 ? currentAngle : originalEulers.z);
    }


}
