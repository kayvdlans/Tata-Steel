using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulateWaterRising : MonoBehaviour
{
    [SerializeField] private float speed;

    private void Update()
    {
        transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
    }
}
