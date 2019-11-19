using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] private Vector2Int size;
    [SerializeField] private float waveSpeed;
    [SerializeField] private float columnSize = 1f;
    [SerializeField] private float scaling = 0.5f;
    [SerializeField] private float splashForce = 5f;
    [SerializeField] private int splashCount = 10;
    [SerializeField] private float maxSlope = 2f;
    [SerializeField] private float baseLevel = 5f;
    [SerializeField] private float updateInterval = 0.05f;

    [SerializeField] private GameObject columnPrefab;
    
    private Transform[,] O;
    private float[,] U0;
    private float[,] U1;
    private float[,] V;
    private float[,] R0;
    private float[,] R1;   

    private float timeLastChecked = 0;
    private List<GameObject> collisionObjects = new List<GameObject>();

    private void Start()
    {
        O = new Transform[size.x, size.y];
        U0 = new float[size.x, size.y];
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                U0[i, j] = baseLevel;
                O[i, j] = Instantiate(columnPrefab, this.transform).transform;
                UpdateObject(i, j);
            }
        }

        U0[size.x / 2, size.y / 2] = splashForce;

        V = new float[size.x, size.y];
        R0 = new float[size.x, size.y];
        R1 = new float[size.x, size.y];
        U1 = new float[size.x, size.y];

        timeLastChecked = Time.time;
        collisionObjects.AddRange(GameObject.FindGameObjectsWithTag("WaterCollisionObjects"));
        
        StartCoroutine(DoSimulation(updateInterval));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            for (int i = 0; i < splashCount; i++)
            {
                U0[Random.Range(0, size.x - 1), Random.Range(0, size.y - 1)] = splashForce;
            }
        }
    }

    private IEnumerator DoSimulation(float time)
    {
        while (true)
        {
            SimulationStep(Time.time - timeLastChecked);
            timeLastChecked = Time.time;

            yield return new WaitForSeconds(time);
        }
    }
    
    private float GetCombinationOfAdjacentHeights(int i, int j, float[,] map)
    {
        float xy = 0;
        xy += i + 1 < size.x ? map[i + 1, j] : 0;
        xy += i - 1 >= 0 ? map[i - 1, j] : 0;
        xy += j + 1 < size.y ? map[i, j + 1] : 0;
        xy += j - 1 >= 0 ? map[i, j - 1] : 0;

        return xy;
    }

    private void UpdateObject(int i, int j)
    {
        O[i, j].localPosition = new Vector3(i * (columnSize / 2), U0[i, j] / 2, j * (columnSize / 2));
        O[i, j].localScale = new Vector3(columnSize, U0[i, j], columnSize);        
    }

    private float ComputeForce(int i, int j, float c, float h)
    {
        return Mathf.Sqrt(c) * (GetCombinationOfAdjacentHeights(i, j, U0) - 4 * U0[i, j]) / Mathf.Sqrt(h);
    }

    private void ComputeCollisionCoefficient(int i, int j, GameObject t)
    {
        Bounds B0 = O[i, j].GetComponent<Renderer>().bounds;
        Bounds B1 = t.GetComponent<Renderer>().bounds;

        R0[i, j] = B1.Intersects(B0) ? B1.min.y - B0.max.y : 0; 
    }

    private bool AdjacentCellContainsObject(int i, int j)
    {
        return GetCombinationOfAdjacentHeights(i, j, R0) != 0;
    }


    private void SimulationStep(float timeStep)
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                float f = ComputeForce(i, j, waveSpeed, columnSize);
                V[i, j] += scaling * (f * timeStep);
                U1[i, j] = U0[i, j] + V[i, j] * timeStep;
            }
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                U0[i, j] = U1[i, j];
                
                if (U0[i, j] < 5.0f) 
                    U0[i, j] = 5.0f; 
                
                for (int k = 0; k < collisionObjects.Count; k++)
                    ComputeCollisionCoefficient(i, j, collisionObjects[k]);

                if (!AdjacentCellContainsObject(i, j))
                    ClampHeight(i, j, columnSize);

                U0[i, j] += R0[i, j];
                
                UpdateObject(i, j);
            }
        }
    }

    private void ClampHeight(int i, int j, float h)
    {
        float offset = GetCombinationOfAdjacentHeights(i, j, U0) / 4 - U0[i, j];
        float maxOffset = maxSlope * h;

        if (offset > maxOffset)
            U0[i, j] += offset - maxOffset;
        if (offset < -maxOffset)
            U0[i, j] += offset + maxOffset;
    }
}