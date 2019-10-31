using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveDispencer : MonoBehaviour
{
    public List<Objectives> objectives = new List<Objectives>();
    
    public List<Text> objectiveText = new List<Text>();
    public List<Text> progressText = new List<Text>();
    public List<Text> doneText = new List<Text>();
    public Text score;
    int reward;
    int maxReward;

    private void Start()
    {
        OpenMenu();

        for (int i = 0; i < objectives.Count; i++)
        {
            maxReward += objectives[i].reward;
        }
    }

    public void Update()
    {
        UpdateMenu();

        score.text = "behaalde score: " + reward.ToString() + "/" + maxReward;
    }

    public void OpenMenu()
    {
        for (int i = 0; i < objectives.Count; i++)
        {
            objectives[i].isActive = true;
            objectiveText[i].text = objectives[i].objective;
        }
    }

    public void UpdateMenu()
    {
        for (int i = 0; i < objectives.Count; i++)
        {
            progressText[i].text = objectives[i].currentAmount.ToString() + " / " + objectives[i].requiredAmount.ToString();
            if (objectives[i].IsReached() && objectives[i].isActive) { doneText[i].text = "Af: Ja";
                reward += objectives[i].reward;
                objectives[i].isActive = false;
            }
            else if (objectives[i].IsReached() == false && objectives[i].isActive) { doneText[i].text = "Af: Nee"; }
            
        }
    }
<<<<<<< Updated upstream
=======
    

>>>>>>> Stashed changes
}
