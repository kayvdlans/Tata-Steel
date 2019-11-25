using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushSessionInfo : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RoomSettings roomSettings;
    [SerializeField] private ObjectiveDispencer objectives;

    [Space]
    [SerializeField][Range(0, 1)] private float passingRatio;

    private int totalMistakes = 0;

    private void Start()
    {
        foreach (Mistake mistake in FindObjectsOfType<Mistake>())
        {
            mistake.AddListener(IncrementMistakeCount);
        }
    }

    private void IncrementMistakeCount()
    {
        totalMistakes++;
    }

    //Get all points
    //Get all mistakes

    /// <summary>
    /// Call this method in the OnStartInteraction (of the door script).
    /// </summary>
    public void SetSessionInfo()
    {
        float time = Time.timeSinceLevelLoad;

        int maxPoints = objectives.MaxPoints;
        int points = objectives.Points;
       
        bool passed = (float)points / maxPoints > passingRatio;

        roomSettings.SetSessionInfo(time, points, totalMistakes, passed);
    }
}
