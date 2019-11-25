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
       
        /*
        List<Mistake> mistakes = new List<Mistakes>();
        for (int i = 0; i < mistakes.Count; i++)
        {
            points -= mistakes.Points; //do this realtime instead? so it already gets changed in the objective dispenser
        }
         */
        bool passed = (float)points / maxPoints > passingRatio;
        int mistakes = 0;
        roomSettings.SetSessionInfo(time, points, mistakes/*.Count*/, passed);
    }
}
