using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drainivalve : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private Transform s;
    [SerializeField] private MathHelper.Axis axis;
    [SerializeField] private Vector2 offOnRotation;
    [SerializeField] private bool startOn;

    public TemperatureControl tempCon;
    private Vector3 originalEulers = Vector3.zero;
    private float currentAngle = 0;

    public bool CurrentlyTurnedOn { get; private set; } = false;

    private void Start()
    {
        originalEulers = s.localEulerAngles;
        UpdateValue(startOn);
        StartCoroutine(CheckIfTempPlus());
    }

    public void Flip()
    {
        UpdateValue(!CurrentlyTurnedOn);
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

    IEnumerator CheckIfTempPlus()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            if (CurrentlyTurnedOn)
            {
                AddTemp();
                Debug.Log("Increasing");
            }
            if (CurrentlyTurnedOn == false && tempCon.CurrentTemperature >= 15)
            {
                LowerTemp();
                Debug.Log("Decreasing");
            }
        }
    }

    void AddTemp()
    {
        tempCon.CurrentTemperature += 1;
    }
    
    void LowerTemp()
    {
        tempCon.CurrentTemperature -= 1;

    }
}


