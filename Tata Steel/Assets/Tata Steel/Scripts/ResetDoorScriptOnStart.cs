using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetDoorScriptOnStart : MonoBehaviour
{
    [SerializeField] private OpenedDoors doors;

    private void Start()
    {
        doors.ResetOpenedDoors(false);    
    }
}
