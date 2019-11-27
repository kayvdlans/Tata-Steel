using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingRoom2ObjectivesCounter : MonoBehaviour
{ 
    public ObjectiveDispencer objectivesDis;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Ventilator()
    {
        if (objectivesDis.objectives[0].isActive)
        {
            objectivesDis.objectives[0].AddCurrent();
        }
    }

    public void ZuigAfsluiter()
    {
        if (objectivesDis.objectives[1].isActive)
        {
            objectivesDis.objectives[1].AddCurrent();
        }
    }

    public void Voedingspomp()
    {
        if (objectivesDis.objectives[2].isActive)
        {
            objectivesDis.objectives[2].AddCurrent();
        }
    }


    public void Vulafsluiter()
    {
        if(objectivesDis.objectives[4].isActive)
        {
            objectivesDis.objectives[4].AddCurrent();
        }
    }

    public void Afsluiters()
    {
        if(objectivesDis.objectives[5].isActive)
        {
            objectivesDis.objectives[5].AddCurrent();
        }
    }
}
