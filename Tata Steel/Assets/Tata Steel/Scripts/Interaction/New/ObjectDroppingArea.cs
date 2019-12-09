using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ObjectDroppingArea : MonoBehaviour
{
    public bool IsValid { get { return objects.Count == 1; } }

    private List<UserTestThrowable> objects = new List<UserTestThrowable>();
    private BoxCollider boxCollider;
    private bool needsLerp = false;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        StartCoroutine(CheckForLerp());
    }

    private IEnumerator LerpObjectToCenter()
    {
        Transform t = objects[0].transform;
        Rigidbody r = objects[0].GetComponent<Rigidbody>();
        r.useGravity = false;
        needsLerp = false;

        Vector3 destination = boxCollider.bounds.center;
        destination.y = t.position.y;
        float originalDistance = (destination - t.position).magnitude;
        float distance;
        float threshold = 0.2f;
        float speed = 0.1f;

        yield return new WaitForSeconds(0.5f);

        while ((distance = (destination - t.transform.position).magnitude) > threshold)
        {
            //ratio is used to smooth the move speed based on the distance to the destination
            float ratio = (distance + threshold) / originalDistance;

            t.position = Vector3.MoveTowards(t.position, destination, speed * ratio);

            yield return new WaitForEndOfFrame();
        }

        r.useGravity = true;
    }

    //Just a cheap update loop since we don't need to check it much at all. 
    private IEnumerator CheckForLerp()
    {
        while (true)
        {
            if (needsLerp)
            {
                StartCoroutine(LerpObjectToCenter());
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("UserTestObject"))
        {
            UserTestThrowable t = other.GetComponent<UserTestThrowable>();
            if (t.LinkedArea == null)
            {
                t.LinkedArea = this;
                objects.Add(other.GetComponent<UserTestThrowable>());
                needsLerp = objects.Count == 1;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("UserTestObject"))
        {
            UserTestThrowable t = other.GetComponent<UserTestThrowable>();
            if (t.LinkedArea == this)
            {
                t.LinkedArea = null;
            }

            objects.Remove(t);
            needsLerp = objects.Count > 0;
        }
    }
}
