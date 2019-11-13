using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorkGearRequirements : MonoBehaviour
{
    [SerializeField]
    private OpenedDoors openedDoors;

    [SerializeField]
    private RoomSettings roomToOpen;

    [SerializeField]
    private List<Interactable> workgear;

    private void Awake()
    {
        if (workgear.Count > 0)
            openedDoors.ResetOpenedDoors(false);
    }

    public void PickUp(Interactable gear)
    {
        workgear.Remove(gear);

        if (workgear.Count == 0)
            openedDoors.OpenDoorByIndex(roomToOpen.RoomID);

        Destroy(gear.gameObject);
    }
}
