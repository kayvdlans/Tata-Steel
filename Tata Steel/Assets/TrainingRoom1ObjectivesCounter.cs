using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingRoom1ObjectivesCounter : MonoBehaviour
{
    public GameObject flask;
    public GameObject stoomafsluiters;
    public GameObject aftapsluiter;
    public VisualCheckChecker VisualCheck;
    public ObjectiveDispencer objectivesDis;
    public string LookingAt;
    // Update is called once per frame
    private void Start()
    {
        flask.tag = "Flask";
        stoomafsluiters.tag = "Stoomafsluiters";
        aftapsluiter.tag = "Aftapsluiter";
    }

    void Update()
    {
        if (VisualCheck.LookingAt == "Drainvalve")
            objectivesDis.objectives[5].AddCurrent(); ;

        if (VisualCheck.LookingAt == "MainFlask")
            objectivesDis.objectives[3].AddCurrent(); ;

        if (VisualCheck.LookingAt == "PressureMechanism")
            objectivesDis.objectives[4].AddCurrent(); ;

    }
}

