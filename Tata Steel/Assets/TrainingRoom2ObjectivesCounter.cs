using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingRoom2ObjectivesCounter : MonoBehaviour
{
    public InternePompButton inter;
    public ObjectiveDispencer objectivesDis;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inter.inter && objectivesDis.objectives[0].isActive)
            objectivesDis.objectives[0].AddCurrent();

        if (inter.inter && objectivesDis.objectives[2].isActive)
            objectivesDis.objectives[2].AddCurrent();
    }
}
