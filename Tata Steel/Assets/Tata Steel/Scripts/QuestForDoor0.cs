using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestForDoor0 : MonoBehaviour
{
    public ObjectiveDispencer objectivesDis;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        objectivesDis.objectives[0].AddCurrent();
    }
}
