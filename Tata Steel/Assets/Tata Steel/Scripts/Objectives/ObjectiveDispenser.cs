using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

public class ObjectiveDispenser : MonoBehaviour
{
    [SerializeField] private Door door;
    [SerializeField] private List<Objective> objectives = new List<Objective>();

    [Header("Teleport Points")]
    [SerializeField] private TeleportPoint.IndicatorType indicatorType;
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

    private void UpdateTeleportVisuals(int index, TeleportPoint point)
    {
        if (point == null)
        {
            return;
        }

        bool isCurrentObjective = false;
        bool isDoor = door.TeleportPoint.Equals(point);
        if (isDoor)
        {
            isCurrentObjective = CurrentObjective == null;
        }
        else if (CurrentObjective != null)
        {
            isCurrentObjective = CurrentObjective.TeleportPoint.Equals(point);
        }

        switch (indicatorType)
        {
            case TeleportPoint.IndicatorType.Color:
                point.teleportType = TeleportPoint.TeleportPointType.None;
                point.title = $"";
                break;
            case TeleportPoint.IndicatorType.Arrow:
                point.teleportType = TeleportPoint.TeleportPointType.MoveToLocation;
                point.title = $"";
                point.onlyShowIconIfActive = true;
                break;
            case TeleportPoint.IndicatorType.Number:
                point.teleportType = TeleportPoint.TeleportPointType.None;
                point.title = $"{index + 1}";
                point.titleVisibleColor = active.GetColor("_TintColor");
                break;
            case TeleportPoint.IndicatorType.IKEA:
                point.teleportType = TeleportPoint.TeleportPointType.None;
                point.title = $"";
                break;
        }

        point.indicatorType = indicatorType;
        point.active = isCurrentObjective;
        point.UpdateVisuals();
    }

    private void Update()
    {
        //Debug Only
        if (Input.GetKeyDown(KeyCode.F) && CurrentObjective != null)
            CurrentObjective.CurrentAmount++;
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
            UpdateTeleportVisuals(i, objectives[i].TeleportPoint);
        }

        UpdateTeleportVisuals(objectives.Count, door.TeleportPoint);

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
            UpdateTeleportVisuals(i, objectives[i].TeleportPoint);
        }

        UpdateTeleportVisuals(objectives.Count, door.TeleportPoint);
        ShowMenu();

        door.Opened = CurrentObjective == null;
    }
}
