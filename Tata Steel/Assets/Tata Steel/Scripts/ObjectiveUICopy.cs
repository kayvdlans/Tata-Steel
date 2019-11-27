using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveUICopy : MonoBehaviour
{
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private ObjectiveDispencer oD;

    private List<Text> objective = new List<Text>();
    private List<Text> progress = new List<Text>();
    private List<Text> done = new List<Text>();
    private Text score = null;

    private void Start()
    {
        StartCoroutine(WaitForInitialize());
    }

    private IEnumerator WaitForInitialize()
    {
        yield return new WaitForSeconds(0.5f);

        objective = InitializeObjectsFromList(oD.objectiveText);
        progress = InitializeObjectsFromList(oD.progressText);
        done = InitializeObjectsFromList(oD.doneText);

        score = Instantiate(oD.score, parentCanvas.transform);

        oD.OnObjectiveDone += UpdateDoneText;
    }

    private List<Text> InitializeObjectsFromList(List<Text> list) 
    {
        List<Text> objects = new List<Text>();

        for (int i = 0; i < list.Count; i++)
        {
            objects.Add(Instantiate(list[i], parentCanvas.transform));
        }

        return objects;
    }

    private void UpdateDoneText(int index)
    {
        done[index].text = "Af: Ja";
    }

    private void Update()
    {
        for (int i = 0; i < objective.Count; i++)
        {
            if (progress.Count > 0)
            {
                progress[i].text = oD.objectives[i].currentAmount + " / " + oD.objectives[i].requiredAmount;
            }
        }

        score.text = "behaalde score: " + oD.Points + "/" + oD.MaxPoints;
    }
}
