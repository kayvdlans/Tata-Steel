using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class analytics : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        Analytics.CustomEvent(Time.realtimeSinceStartup.ToString());
    }
}