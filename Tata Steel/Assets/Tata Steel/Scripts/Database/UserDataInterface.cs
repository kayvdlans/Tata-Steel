using UnityEngine;
using UnityEngine.UI;

public class UserDataInterface : MonoBehaviour
{
    [SerializeField] private UserData userData;
    [SerializeField] private GameObject userIDObject;
    [SerializeField] private GameObject[] dataObjects;

    private void Start()
    {
        userData.OnUpdateUserData += UpdateUIElements;
    }

    private void UpdateUIElements()
    {
        userIDObject.GetComponent<Text>().text = "User: #" + userData.User.ID;

        foreach (GameObject data in dataObjects)
        {
            Text t = data.transform.GetChild(0).GetComponent<Text>();
            switch (data.GetComponent<InterfaceData>().DataType)
            {
                case InterfaceData.Type.Time:
                    t.text = userData.User.TotalTime.ToString();
                    break;
                case InterfaceData.Type.Points:
                    t.text = userData.User.TotalPoints.ToString();
                    break;
                case InterfaceData.Type.Mistakes:
                    t.text = userData.User.TotalMistakes.ToString();
                    break;
                case InterfaceData.Type.Attempts:
                    t.text = userData.User.TotalAttempts.ToString();
                    break;
            }
        }
    }
}
