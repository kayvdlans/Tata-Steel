using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpColor : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    private float speed;

    [SerializeField]
    private Gradient gradient;

    private Renderer r;

    private void Start()
    {
        r = GetComponent<Renderer>();
    }

    private void Update()
    {
        r.material.color = gradient.Evaluate((Time.time * speed) % 1);
    }
}
