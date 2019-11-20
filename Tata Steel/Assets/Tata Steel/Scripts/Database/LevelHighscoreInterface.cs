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
    [SerializeField] private GameObject passedDataObject;
    [SerializeField] private GameObject[] dataObjects;

    private void Start()
    {
        userData.OnUpdateHighscores += UpdateUIElements;
    }

    private void UpdateUIElements()
    {
        List<SessionInfo> passedSessions = new List<SessionInfo>();
        passedSessions.AddRange(userData.Sessions.Where(s => s.Passed && s.LevelID == roomSettings.RoomID));
        passedDataObject.SetActive(passedSessions.Count != 0);

        List<LevelInfo> info = new List<LevelInfo>();
        info.AddRange(userData.Highscores.Where(s => s.LevelID == roomSettings.RoomID));

        noDataObject.SetActive(info.Count == 0);

        foreach (GameObject data in dataObjects)
        {
            data.SetActive(info.Count != 0);

            if (info.Count != 0)
            {
                Text t = data.transform.GetChild(0).GetComponent<Text>();
                switch (data.GetComponent<InterfaceData>().DataType)
                {
                    case InterfaceData.Type.Time:
                        t.text = info[0].BestTime.ToString();
                        break;
                    case InterfaceData.Type.Points:
                        t.text = info[0].HighestPoints.ToString();
                        break;
                    case InterfaceData.Type.Mistakes:
                        t.text = info[0].LowestMistakes.ToString();
                        break;
                    case InterfaceData.Type.Attempts:
                        t.text = info[0].TotalAttempts.ToString();
                        break;
                }
            }
        }
    }
}
