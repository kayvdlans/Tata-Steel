using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Quests
{
    protected static List<Quest> questList = new List<Quest>();
    public static List<Quest> GetQuests() { return new List<Quest>(questList); }
    public abstract bool FinishQuest();
    public abstract string GetDescription();
    public abstract string GetObjective();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    

}
