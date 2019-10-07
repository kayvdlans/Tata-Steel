using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorkGearRequirements : MonoBehaviour
{
    [SerializeField]
    private OpenedDoors openedDoors;

    [SerializeField]
    private List<Interactable> workgear;

    [SerializeField]
    private UnityEvent onRequirementsMet;

    private void Awake()
    {
        if (workgear.Count > 0)
            openedDoors.ResetOpenedDoors(false);
    }

    public void PickUp(Interactable gear)
    {
        workgear.Remove(gear);
        if (workgear.Count == 0)
            onRequirementsMet.Invoke();
        Destroy(gear.gameObject);
    }
}
