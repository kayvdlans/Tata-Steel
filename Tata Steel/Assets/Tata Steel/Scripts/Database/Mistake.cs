using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Mistake : MonoBehaviour
{
    //haha Im a mistake

    [Header("Linked Objective")]
    [SerializeField] private int index;
    [SerializeField] private ObjectiveDispencer objectives;
    [Header("Mistake Information")]
    [SerializeField] private string mistakeID; 
    [SerializeField] [Range(0, 1)] private float pointReductionRatio;
    [SerializeField] private bool repeatableMistake;

    private bool mistakeMade = false;

    //Add functionalities to this action.
    private UnityAction onMistakeMade;

    private void Start()
    {
        if (!objectives)
            throw new NullReferenceException();

        AddListener(UpdatePoints);
    }

    private void UpdatePoints()
    {
        Objectives o = objectives.objectives[index];
        if (o != null)
        {
            o.reward = (int)(o.reward * (1.0f - pointReductionRatio));
        }
    }

    //Call this whenever a mistake is made. :)
    public void MakeMistake()
    {
        if (!mistakeMade || repeatableMistake)
            if (onMistakeMade != null)
                onMistakeMade.Invoke();
    }

    public void AddListener(UnityAction listener)
    {
        onMistakeMade += listener;
    }    

    public static Mistake GetMistakeByID(string id)
    {
        Mistake[] mistakes = (FindObjectsOfType(typeof(Mistake)) as Mistake[]).Where(m => m.mistakeID == id).ToArray();

        if (mistakes.Count() > 1)
        {
            Debug.LogError("There are multiple mistakes with ID: " + id + ".");
            return null;
        }
        else if (mistakes.Count() == 1)
        {
            return mistakes[0];
        }

        Debug.LogError("There are no mistakes with ID: " + id + ".");
        return null;
    }
}
