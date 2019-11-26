using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveRewardsForValves : MonoBehaviour
{
    public ObjectiveDispencer obj;
    public void ValveCheck()
    {        if(obj.objectives[5].isActive)
        obj.objectives[5].AddCurrent();
    }

    public void StoomGenerator()
    {
        if(obj.objectives[1].isActive)
        obj.objectives[1].AddCurrent();
    }

    public void StoomAfsluiters()
    {
        if (obj.objectives[4].isActive)
        {
            obj.objectives[4].AddCurrent();
        }
    }
}
