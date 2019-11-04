using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveOnCollision : MonoBehaviour
{
    public ObjectiveDispencer objectivesDis;
    [SerializeField] private Collider colliderObject;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.Equals(colliderObject))
        {
            objectivesDis.objectives[0].AddCurrent();
            Destroy(gameObject);
        }
    }
}
