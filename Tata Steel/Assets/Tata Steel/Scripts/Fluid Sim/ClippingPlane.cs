using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClippingPlane : MonoBehaviour
{
    public enum Index
    {
        Zero,
        One,
        Two,
        Three,
        Four
    }

    [SerializeField] private Index index;
    [SerializeField] private Material material;

    void Update()
    {
        Plane plane = new Plane(transform.up, transform.position);

        Vector4 p = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);

        material.SetVector("_Plane" + (int) index, p);
    }
}