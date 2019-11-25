using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OpenedDoors", menuName = "ScriptableObjects/OpenedDoors")]
public class OpenedDoors : ScriptableObject
{
    [SerializeField]
    private RoomSettings firstDoor;

    [SerializeField]
    private List<int> openedDoorsByIndex;

    public void OpenDoorByIndex(int index)
    {
        if (!openedDoorsByIndex.Contains(index))
            openedDoorsByIndex.Add(index);
        else
            Debug.LogWarning("Index is already added to List");
    }

    public bool IsAdded(int index)
    {
        return openedDoorsByIndex.Contains(index);
    }

    public void CloseDoorByIndex(int index)
    {
        if (openedDoorsByIndex.Contains(index))
            openedDoorsByIndex.Remove(index);
        else
            Debug.LogWarning("Index is currently not in the List");
    }

    public void ResetOpenedDoors(bool firstDoorOpened)
    {
        openedDoorsByIndex.Clear();
        if (firstDoor != null && firstDoorOpened)
            openedDoorsByIndex.Add(firstDoor.RoomID);
    }
}
