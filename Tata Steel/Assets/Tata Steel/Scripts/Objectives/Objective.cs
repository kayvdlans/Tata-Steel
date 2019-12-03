using System;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

[Serializable]
public class Objective
{
    [SerializeField][TextArea]
    private string objectiveText;
    public string ObjectiveText { get => objectiveText; }

    [SerializeField]
    private int reward;
    public int Reward { get => reward; set => reward = value; }

    [SerializeField]
    private float requiredAmount;
    public float RequiredAmount { get => requiredAmount; }

    private float currentAmount = 0;
    public float CurrentAmount
    {
        get => currentAmount;
        set
        {
            currentAmount = value;
            if (OnUpdateCurrent != null)
            {
                OnUpdateCurrent.Invoke();
            }
        }
    }

    [SerializeField]
    private TeleportPoint teleportPoint;
    public TeleportPoint TeleportPoint { get => teleportPoint; }

    public bool AmountReached  { get => CurrentAmount >= requiredAmount; }
    public bool Done { get; set; } = false;

    public UnityAction OnUpdateCurrent { get; set; }
}