using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class finalExamObjective : MonoBehaviour
{
    public FlipSwitch flip;
    public ObjectiveDispencer objectivesDis;
    public PressureBuildup pressure;
    public TemperatureControl tempCon;
    // Start is called before the first frame update

    public void Update()
    {
            objectivesDis.objectives[1].currentAmount = pressure.CurrentValue;
     
        if (flip.CurrentlyTurnedOn == false)
        {
            objectivesDis.objectives[3].AddCurrent();
        }
            objectivesDis.objectives[2].currentAmount = tempCon.CurrentTemperature;
        
    }
}
