using UnityEngine;
using System.Collections;

/// <summary>
/// Currently just use Flip() to turn the machine on or off using the Interactable script
/// </summary>
public class FlipSwitch : MonoBehaviour
{
    [SerializeField] private MathHelper.Axis axis;
    [SerializeField] private Vector2 offOnRotation;
    [SerializeField] private bool startOn;
    [SerializeField] private GameObject objectToActivate;

    private CustomInteraction customInteraction;
    private Vector3 originalEulers = Vector3.zero;
    private float currentAngle = 0;

    
    public bool CurrentlyTurnedOn { get; private set; } = false;

    private void Start()
    {
        customInteraction = GetComponent<CustomInteraction>();
        customInteraction.OnStartInteraction += Flip;

        originalEulers = transform.localEulerAngles;
        UpdateValue(startOn);
    }

    public void Flip()
    {
        UpdateValue(!CurrentlyTurnedOn);
        objectToActivate.SetActive(objectToActivate != null && !objectToActivate.activeSelf);
    }

    private void UpdateValue(bool on)
    {
        CurrentlyTurnedOn = on;
        currentAngle = on? offOnRotation.y : offOnRotation.x;

        transform.localEulerAngles = new Vector3(
            (int) axis == 0 ? currentAngle : originalEulers.x,
            (int) axis == 1 ? currentAngle : originalEulers.y,
            (int) axis == 2 ? currentAngle : originalEulers.z);
    }
}