using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class questfordoor2 : MonoBehaviour
{
    public ObjectiveDispencer objectivesDis;
    // Start is called before the first frame update
    void Start()
    {
        objectivesDis.objectives[0].AddCurrent();
    }

    // Update is called once per frame
    void Update()
    { } 
}
