using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureControl : MonoBehaviour
{
    [SerializeField] private Window_Graph temperatureGraph = null;
    [SerializeField] private float graphUpdateInterval = 1f;
    [Space]
    [SerializeField] private int baseTemperature = 50;
    [SerializeField] private bool lockTemperatureBetweenBounds = false;
    [SerializeField] private Vector2Int temperatureBounds = new Vector2Int(0, 100);

    public int BaseTemperature { get { return baseTemperature; } }
    public int CurrentTemperature { get; private set; } = 0;

    private void Start()
    {
        CurrentTemperature = lockTemperatureBetweenBounds ? 
            MathHelper.ConfineToBounds(BaseTemperature, temperatureBounds) : BaseTemperature;

        temperatureGraph.GraphValues.Clear();
        StartCoroutine(UpdateGraphVisual(graphUpdateInterval));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            UpdateTemperature(1);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            UpdateTemperature(-1);
    }

    public void UpdateTemperature(int change) 
    {
        CurrentTemperature += change;

        //Added the option to confine in case we want to lock it at a certain point
        //but also kept the option to not confine in case we want to make things happen we the temperature
        //rises too much
        if (lockTemperatureBetweenBounds)
        {
            CurrentTemperature = MathHelper.ConfineToBounds(CurrentTemperature, temperatureBounds);
        }
    }

    private IEnumerator UpdateGraphVisual(float interval)
    {
        while (true)
        {
            temperatureGraph.GraphValues.Add(CurrentTemperature);
            temperatureGraph.SetGraphVisual();

            yield return new WaitForSeconds(interval);
        }
    }
}
