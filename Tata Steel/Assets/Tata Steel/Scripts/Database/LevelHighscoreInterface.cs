using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelHighscoreInterface : MonoBehaviour
{
    [SerializeField] private RoomSettings roomSettings;
    [SerializeField] private UserData userData;
    [Space]
    [SerializeField] private GameObject noDataObject;
    [SerializeField] private GameObject[] dataObjects;

    private void Start()
    {
        userData.OnUpdateHighscores += UpdateUIElements;
    }

    private void UpdateUIElements()
    {
        List<LevelInfo> info = new List<LevelInfo>();
        info.AddRange(userData.Highscores.Where(s => s.LevelID == roomSettings.RoomID));

        noDataObject.SetActive(info.Count == 0);

        foreach (GameObject data in dataObjects)
        {
            data.SetActive(info.Count != 0);

            if (info.Count != 0)
            {
                Text t = data.transform.GetChild(0).GetComponent<Text>();
                switch (data.GetComponent<InterfaceDataType>().DataType)
                {
                    case InterfaceDataType.Type.Time:
                        t.text = info[0].BestTime.ToString();
                        break;
                    case InterfaceDataType.Type.Points:
                        t.text = info[0].HighestPoints.ToString();
                        break;
                    case InterfaceDataType.Type.Mistakes:
                        t.text = info[0].LowestMistakes.ToString();
                        break;
                    case InterfaceDataType.Type.Attempts:
                        t.text = info[0].TotalAttempts.ToString();
                        break;
                }
            }
        }
    }
}
