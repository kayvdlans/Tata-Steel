using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Objectives
{
    public bool isActive;

    public string objective;
    public int reward;

    public int requiredAmount;
    public int currentAmount;



    public bool IsReached()
    {
        return (currentAmount >= requiredAmount);

    }

    public void AddCurrent()
    {
        currentAmount++;
    }
}