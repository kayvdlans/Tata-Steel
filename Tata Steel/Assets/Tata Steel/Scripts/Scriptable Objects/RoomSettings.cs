using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "new RoomSettings", menuName = "ScriptableObjects/RoomSettings")]
public class RoomSettings : ScriptableObject
{
    [SerializeField] private int roomID;
    [SerializeField] private string sceneName;   
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private OpenedDoors openedDoors;
    [SerializeField] private List<RoomSettings> doorsToOpen;
    [SerializeField] private UnityEvent onRoomCompleted;
    [SerializeField] private UserData userData;

    private SessionInfo sessionInfo;

    public int RoomID { get => roomID; }
    public string SceneName { get => sceneName; }

    public void SetSessionInfo(float time, int points, int mistakes, bool passed)
    {
        sessionInfo = new SessionInfo()
        {
            SessionID   = (uint)userData.SessionsAmount,
            LevelID     = (uint)roomID,
            Time        = (uint)time,
            Points      = (uint)points,
            Mistakes    = (uint)mistakes,
            Passed      = passed,
            UserID      = userData.ID
        };
    }

    public void RoomCompleted()
    {
        foreach (RoomSettings door in doorsToOpen)
        {
            openedDoors.OpenDoorByIndex(door.RoomID);
        }

        onRoomCompleted.Invoke();        

        userData.AddSession(sessionInfo);
        
        sceneLoader.LoadScene("Entrance Hall");
    }
}
