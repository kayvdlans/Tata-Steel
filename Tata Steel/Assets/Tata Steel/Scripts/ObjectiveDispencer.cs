using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ObjectiveDispencer : MonoBehaviour
{
    public List<Objectives> objectives = new List<Objectives>();
    public List<Text> objectiveText = new List<Text>();
    public List<Text> progressText = new List<Text>();
    public List<Text> doneText = new List<Text>();
    public Text score;
    public Door door;

    public int Points { get; private set; }
    public int MaxPoints { get; private set; }
    public UnityAction<int> OnObjectiveDone { get; set; }

    private void Start()
    {
        OpenMenu();

        for (int i = 0; i < objectives.Count; i++)
        {
            MaxPoints += objectives[i].reward;
        }
    }

    public void Update()
    {
        UpdateMenu();
        
        for (int i = 0; i < objectives.Count; i++)
        {
            objectives[i].IsReached();
        }
        if(score!=null)
        score.text = "behaalde score: " + Points.ToString() + "/" + MaxPoints;
    }

    public void OpenMenu()
    {
        for (int i = 0; i < objectives.Count; i++)
        {
            objectives[i].isActive = true;
            if(objectiveText.Count > 0)
                objectiveText[i].text = objectives[i].objective;
        }
    }

    public void UpdateMenu()
    {
        bool newBool = true;

        for (int i = 0; i < objectives.Count; i++)
        {
            if(progressText.Count > 0)
            progressText[i].text = objectives[i].currentAmount.ToString() + " / " + objectives[i].requiredAmount.ToString();
            if (objectives[i].IsReached() && objectives[i].isActive) {
                if(doneText.Count > 0)
                doneText[i].text = "Af: Ja";

                if (OnObjectiveDone != null)
                    OnObjectiveDone.Invoke(i);

                Points += objectives[i].reward;
                objectives[i].isActive = false;
            }
            else if (objectives[i].IsReached() == false && objectives[i].isActive)
            {                
                if(doneText.Count > 0)
                doneText[i].text = "Af: Nee";
                newBool = false;
            }
         
        }

        if (newBool)
        {
            door.Opened = true;
        }
        
    }
}
