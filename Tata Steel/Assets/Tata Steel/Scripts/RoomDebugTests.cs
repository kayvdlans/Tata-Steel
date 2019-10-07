using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDebugTests : MonoBehaviour
{
    [SerializeField]
    private bool openFirstDoor = true;

    [SerializeField]
    private OpenedDoors openedDoors = null;

    [SerializeField]
    private List<RoomSettings> roomSettings = null;

    private int currentIndex = 0;

    private void Start()
    {
        openedDoors.ResetOpenedDoors(openFirstDoor);
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            Debug.Log(currentIndex);
            CompleteRoom();
        }
    }

    private void CompleteRoom()
    {
        if (currentIndex < roomSettings.Count)
        roomSettings[currentIndex++].RoomCompleted();
        Door[] doords = FindObjectsOfType(typeof(Door)) as Door[];
        foreach (Door door in doords)
            door.Opened = openedDoors.IsAdded(door.Index);
    }
}
