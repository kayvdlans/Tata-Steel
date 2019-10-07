using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "new RoomSettings", menuName = "ScriptableObjects/RoomSettings")]
public class RoomSettings : ScriptableObject
{
    [SerializeField]
    private OpenedDoors openedDoors;

    [SerializeField]
    private List<int> doorsToOpen;

    [SerializeField]
    private UnityEvent onRoomCompleted;

    public void RoomCompleted()
    {
        foreach (int door in doorsToOpen)
        {
            openedDoors.OpenDoorByIndex(door);
        }

        onRoomCompleted.Invoke();
    }
}
