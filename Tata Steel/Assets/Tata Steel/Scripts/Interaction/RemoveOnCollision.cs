using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveOnCollision : MonoBehaviour
{
    [SerializeField] private Collider colliderObject;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.Equals(colliderObject))
        {
            Destroy(gameObject);
        }
    }
}
