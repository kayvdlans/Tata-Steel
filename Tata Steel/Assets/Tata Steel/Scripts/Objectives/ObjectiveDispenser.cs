﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

public class ObjectiveDispenser : MonoBehaviour
{
    public enum IndicatorType
    {
        Color,
        Number,
        Arrow,
        IKEA
    }

    [SerializeField] private Door door;
    [SerializeField] private List<Objective> objectives = new List<Objective>();

    [Header("Teleport Points")]
    [SerializeField] private IndicatorType indicatorType;
    [SerializeField] private Material active;
    [SerializeField] private Material inactive;

    [Header("Text Objects")]
    [SerializeField] private Text description;
    [SerializeField] private Text progress;
    [SerializeField] private Text score;
    [SerializeField] private Text finished;
  
    public Objective CurrentObjective
    {
        get
        {
            for (int i = 0; i < objectives.Count; i++)
            {
                if (!objectives[i].Done)
                {
                    return objectives[i];
                }
            }

            return null;
        }
    }

    public int Points { get; private set; }
    public int MaxPoints { get; private set; }
    public UnityAction<int> OnObjectiveDone { get; set; }

    private void UpdateTeleportVisuals(TeleportPoint point)
    {
        bool isCurrentObjective = false;
        if (door.TeleportPoint.Equals(point))
        {
            isCurrentObjective = CurrentObjective == null;
        }
        else if (CurrentObjective != null)
        {
            isCurrentObjective = CurrentObjective.TeleportPoint.Equals(point);
        }

        point.active = isCurrentObjective;
        point.titleVisibleColor = isCurrentObjective ? active.GetColor("_TintColor") : inactive.GetColor("_TintColor");
        point.UpdateVisuals();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && CurrentObjective != null)
            CurrentObjective.CurrentAmount++;

        Debug.Log(door.Opened);
    }


    private void Start()
    {
        Teleport.instance.pointActiveVisibleMaterial = active;
        Teleport.instance.pointInactiveVisibleMaterial = inactive;

        for (int i = 0; i < objectives.Count; i++)
        {
            MaxPoints += objectives[i].Reward;
            objectives[i].Done = false;
            objectives[i].OnUpdateCurrent += UpdateMenu;
            UpdateTeleportVisuals(objectives[i].TeleportPoint);
        }

        UpdateTeleportVisuals(door.TeleportPoint);

        ShowMenu();
    }

    public void ShowMenu()
    {
        if (CurrentObjective != null) 
        {
            description.text = CurrentObjective.ObjectiveText;
            progress.text = $"{CurrentObjective.CurrentAmount} / {CurrentObjective.RequiredAmount}";
            finished.text = "";
        }
        else
        {
            description.text = "";
            progress.text = "";
            finished.text = "Alle opdrachten zijn voltooid.\nDe deur is nu open.";
        }

        score.text = $"Punten: {Points} / {MaxPoints}";
    }

    public void UpdateMenu()
    {
        if (CurrentObjective != null)
        {
            int objectiveIndex = objectives.IndexOf(CurrentObjective);

            if (CurrentObjective.AmountReached)
            {
                if (OnObjectiveDone != null)
                {
                    OnObjectiveDone.Invoke(objectiveIndex);
                }

                Points += CurrentObjective.Reward;
                CurrentObjective.Done = true;
            }

        }

        for (int i = 0; i < objectives.Count; i++)
        {
            UpdateTeleportVisuals(objectives[i].TeleportPoint);
        }

        UpdateTeleportVisuals(door.TeleportPoint);
        ShowMenu();

        door.Opened = CurrentObjective == null;
    }
}
