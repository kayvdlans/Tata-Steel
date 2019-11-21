using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] private Vector2Int size;
    [SerializeField] private float columnSize = 1f;
    [SerializeField] private int baseLevel = 5;
    [Space]
    [SerializeField] private float waveSpeed;
    [SerializeField] private float scaling = 1f;
    [SerializeField] private float maxSlope = 2f;
    [SerializeField] private float updateInterval = 0.05f;
    [Space]
    [SerializeField] private float splashForce = 5f;
    [SerializeField] private int splashCount = 10;
    [SerializeField] private float splashingInterval = 1f;
    
    private float[,] U0;
    private float[,] U1;
    private float[,] V;
    private float[,] R0;
    private float[,] R1;   

    private float timeLastChecked = 0;
    private List<GameObject> collisionObjects = new List<GameObject>();

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    private void GenerateMesh()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh(); 
        mesh.name = "Water_Mesh";
        
        int cornerVertices = 8;
        int edgeVertices = (size.x + size.y + baseLevel - 3) * 4;
        int faceVertices = (
            (size.x - 1) * (size.y - 1) +
            (size.x - 1) * (baseLevel - 1) +
            (size.y - 1) * (baseLevel - 1)) * 2;
        vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];

        int v = 0;
        for (int y = 0; y <= baseLevel; y++)
        {
            for (int x = 0; x <= size.x; x++)
                vertices[v++] = new Vector3(x, y, 0);

            for (int z = 1; z <= size.y; z++)
                vertices[v++] = new Vector3(size.x, y, z);

            for (int x = size.x - 1; x >= 0; x--)
                vertices[v++] = new Vector3(x, y, size.y);

            for (int z = size.y - 1; z > 0; z--)
                vertices[v++] = new Vector3(0, y, z);
        }

        for (int z = 1; z < size.y; z++)
            for (int x = 1; x < size.x; x++)
                vertices[v++] = new Vector3(x, baseLevel, z);

        for (int z = 1; z < size.y; z++)
            for (int x = 1; x < size.x; x++)
                vertices[v++] = new Vector3(x, 0, z);

        mesh.vertices = vertices;

        int quads = (size.x * baseLevel + size.x * size.y + baseLevel * size.y) * 2;
        int[] triangles = new int[quads * 6 + 6];
        int ring = (size.x + size.y) * 2;
        int t = 0; v = 0;
        for (int y = 0; y < baseLevel; y++, v++)
        {
            for (int q = 0; q < ring - 1; q++, v++)
            {
                t = SetQuad(triangles, t, v, v + 1, v + ring, v + ring + 1);
            }
            t = SetQuad(triangles, t, v, v - ring + 1, v + ring, v + 1);
        }
        t = CreateTopFace(triangles, t, ring);
        t = CreateBottomFace(triangles, t, ring);
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private int CreateTopFace(int[] triangles, int t, int ring)
    {
        int v = ring * baseLevel;
        for (int x = 0; x < size.x - 1; x++, v++)
        {
            t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
        }
        t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);
        
        int vMin = ring * (baseLevel + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;

        t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + size.x - 1);
        for (int z = 1; z < size.y - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + size.x - 1);
            for (int x = 1; x < size.x - 1; x++, vMid++)
            {
                t = SetQuad(
                    triangles, t,
                    vMid, vMid + 1, vMid + size.x - 1, vMid + size.x);
            }
            t = SetQuad(triangles, t, vMid, vMax, vMid + size.x - 1, vMax + 1);
        }

        int vTop = vMin - 2;
        t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMin - 2);
        for (int x = 1; x < size.x - 1; x++, vTop--, vMid++)
        {
            t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
        }
        t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);

        return t;
    }

    private int CreateBottomFace(int[] triangles, int t, int ring)
    {
        int v = 1;
        int vMid = vertices.Length - (size.x - 1) * (size.y - 1);
        t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
        for (int x = 1; x < size.x - 1; x++, v++, vMid++)
        {
            t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
        }
        t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

        int vMin = ring - 2;
        vMid -= size.x - 2;
        int vMax = v + 2;

        for (int z = 1; z < size.y - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(triangles, t, vMin, vMid + size.x - 1, vMin + 1, vMid);
            for (int x = 1; x < size.x - 1; x++, vMid++)
            {
                t = SetQuad(
                    triangles, t,
                    vMid + size.x - 1, vMid + size.x, vMid, vMid + 1);
            }
            t = SetQuad(triangles, t, vMid + size.x - 1, vMax + 1, vMid, vMax);
        }

        int vTop = vMin - 1;
        t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
        for (int x = 1; x < size.x - 1; x++, vTop--, vMid++)
        {
            t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
        }
        t = SetQuad(triangles, t, vTop , vTop - 1, vMid, vTop - 2);

        return t;
    }

    private int SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
    {
        triangles[i] = v00;
        triangles[i + 1] = triangles[i + 4] = v01;
        triangles[i + 2] = triangles[i + 3] = v10;
        triangles[i + 5] = v11;
        return i + 6;
    }

    private void UpdateMesh()
    {
        int ring = (size.x + size.y) * 2;
        int v = ring * baseLevel;
        
        for (int i = 0, y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++, i++)
            {
                float z = x >= 0 && x < size.x && y >= 0 && y < size.y ? U0[x, y] : 0;
                vertices[v+i].y = z;
            }
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }


    private IEnumerator Splash(float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);

            U0[Random.Range(0, size.x - 1), Random.Range(0, size.y - 1)] = splashForce;
        }
    }

    private void Start()
    {
        U0 = new float[size.x, size.y];

        GenerateMesh();

        V = new float[size.x, size.y];
        R0 = new float[size.x, size.y];
        R1 = new float[size.x, size.y];
        U1 = new float[size.x, size.y];

        timeLastChecked = Time.time;
        collisionObjects.AddRange(GameObject.FindGameObjectsWithTag("WaterCollisionObjects"));
        StartCoroutine(DoSimulation(updateInterval));
        StartCoroutine(Splash(splashingInterval));
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
            UpdateMesh();

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

    private float ComputeForce(int i, int j, float c, float h)
    {
        return Mathf.Sqrt(c) * (GetCombinationOfAdjacentHeights(i, j, U0) - 4 * U0[i, j]) / Mathf.Sqrt(h);
    }

    /*private void ComputeCollisionCoefficient(int i, int j, GameObject t)
    {
        Bounds B0 = O[i, j].bounds;
        Bounds B1 = t.GetComponent<Renderer>().bounds;

        R0[i, j] = B1.Intersects(B0) ? B1.min.y - B0.max.y : R0[i, j]; 
    }*/

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
                
                if (U0[i, j] < baseLevel) 
                    U0[i, j] = baseLevel;

                //R0[i, j] = 0;                
                //for (int k = 0; k < collisionObjects.Count; k++)
                    //ComputeCollisionCoefficient(i, j, collisionObjects[k]);
               // U0[i, j]+= R0[i, j];

                if (!AdjacentCellContainsObject(i, j))
                {
                    ClampHeight(i, j, columnSize);
                    if (U0[i, j] < baseLevel)
                        U0[i, j] = baseLevel;
                }
                else
                {
                    if (U0[i, j] < 0.01f)
                        U0[i, j] = 0.01f;
                }
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