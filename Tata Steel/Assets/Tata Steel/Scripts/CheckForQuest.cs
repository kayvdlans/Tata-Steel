using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckForQuest : MonoBehaviour
{
    public Quest quest;

    public Text objective;
    public Text discription;
    public Text score;
    public GameObject pauseMenu;
    int reward = 0;

    private void Start()
    {
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            OpenMenu();
        }

        score.text = reward.ToString() + " /500";
    }

    public void OpenMenu()
    {
        quest.isActive = true;
        pauseMenu.SetActive(true);
        objective.text = quest.objective;
        discription.text = quest.discription;
    }

    public void CloseMenu()
    {
        pauseMenu.SetActive(false);
        quest.goal.Worked();
        if (quest.goal.IsReached())
        {
            reward = quest.reward;
            quest.isActive = false;
        }
    }

}
