using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ObjectGoal
{
    public ObjectType objectType;
    public int requiredAmount;
    public int currentAmount;
    

    public bool IsReached()
    {
        return (currentAmount >= requiredAmount);
    }
    
    public void Worked()
    {
        if(objectType == ObjectType.Work)
        {
            currentAmount++;
        }
    }
}


public enum ObjectType
{
    Work,
    Clean,
    Things
}