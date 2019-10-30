using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public bool isActive;

    public string objective;
    public string discription;
    public int reward;



    public ObjectGoal goal;
}