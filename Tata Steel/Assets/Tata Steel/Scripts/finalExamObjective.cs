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
        if(pressure.currentValue>=pressure.bounds.y)
        {
            objectivesDis.objectives[1].AddCurrent();
        }

        if (flip.CurrentlyTurnedOn == false)
        {
            objectivesDis.objectives[2].AddCurrent();
        }
            objectivesDis.objectives[2].currentAmount = tempCon.CurrentTemperature;
        
    }
}
