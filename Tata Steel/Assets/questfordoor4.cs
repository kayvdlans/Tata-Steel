using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class questfordoor4 : MonoBehaviour
{
    public PressureBuildup pressurebuild; public ObjectiveDispencer objectivesDis;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(pressurebuild.CurrentValue>=2.5f)
        {
            objectivesDis.objectives[0].AddCurrent();
        }
    }
}
