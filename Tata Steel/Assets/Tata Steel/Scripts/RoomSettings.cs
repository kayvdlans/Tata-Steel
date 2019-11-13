using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "new RoomSettings", menuName = "ScriptableObjects/RoomSettings")]
public class RoomSettings : ScriptableObject
{
    [SerializeField]
    private int roomID;
    public int RoomID { get { return roomID; } }

    [SerializeField]
    private string sceneName;
    public string SceneName { get { return sceneName; } }

    [SerializeField]
    private SceneLoader sceneLoader;

    [SerializeField]
    private OpenedDoors openedDoors;

    [SerializeField]
    private List<RoomSettings> doorsToOpen;

    [SerializeField]
    private UnityEvent onRoomCompleted;

    public void RoomCompleted()
    {
        foreach (RoomSettings door in doorsToOpen)
        {
            openedDoors.OpenDoorByIndex(door.RoomID);
        }

        onRoomCompleted.Invoke();

        sceneLoader.LoadScene("Entrance Hall");
    }
}
