using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternePompButton : MonoBehaviour
{
    public bool inter=false;

    private void OnTriggerEnter(Collider other)
    {
        inter = !inter;
    }

}
