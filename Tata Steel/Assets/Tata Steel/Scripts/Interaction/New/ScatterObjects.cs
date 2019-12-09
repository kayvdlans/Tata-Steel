using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterObjects : MonoBehaviour
{
    [SerializeField] private List<GameObject> objects;
    [Header("Scatter Preparation")]
    [SerializeField] private Collider scatterArea;
    [SerializeField] private float timeBetweenScattering;
    [SerializeField] private float scatterOffsetY;
    [Header("Object Manipulation")]
    [SerializeField] [Range(0.01f, 1f)] private float objectMoveTreshold;
    [SerializeField] [Range(0.01f, 1f)] private float objectMoveSpeed;

    private UserTestTimer timer;
    private ObjectInteractionState interactionState;

    private void Start()
    {
        interactionState = FindObjectOfType<ObjectInteractionState>();
        timer = FindObjectOfType<UserTestTimer>();
        timer.OnTimerFinished += Scatter;
    }

    private void Scatter()
    {
        StopAllCoroutines();
        StartCoroutine(ScatterObject(timeBetweenScattering));
    }

    private IEnumerator ScatterObject(float time)
    {
        foreach (GameObject ob in objects)
        {
            yield return new WaitForSeconds(time);

            float randomX = Random.Range(scatterArea.bounds.min.x, scatterArea.bounds.max.x);
            float randomZ = Random.Range(scatterArea.bounds.min.z, scatterArea.bounds.max.z);
            Vector3 destination = new Vector3(randomX, ob.transform.position.y + scatterOffsetY, randomZ);
            yield return StartCoroutine(MoveObject(ob, destination, objectMoveSpeed));
        }

        interactionState.UpdateState(ObjectInteractionState.State.Grabbable);
    }

    private IEnumerator MoveObject(GameObject ob, Vector3 destination, float speed)
    {
        Rigidbody r = ob.GetComponent<Rigidbody>();
        r.useGravity = false;

        float originalDistance = (destination - ob.transform.position).magnitude;
        float distance;
        while ((distance = (destination - ob.transform.position).magnitude) > objectMoveTreshold)
        {
            float ratio = (distance + objectMoveTreshold) / originalDistance;

            ob.transform.position = Vector3.MoveTowards(ob.transform.position, destination, speed * ratio);

            yield return new WaitForEndOfFrame();
        }

        r.useGravity = true;
    }

    private void OnDrawGizmos()
    {
        if (scatterArea == null)
            return;

        Gizmos.color = Color.cyan;
        Vector3 bottom = scatterArea.bounds.center;
        bottom.y = scatterArea.bounds.min.y;
        Vector3 downscaledY = scatterArea.bounds.size;
        downscaledY.y = 0.01f;
        Gizmos.DrawCube(bottom, downscaledY);   //add a plane to bottom for clarity.
        Gizmos.DrawWireCube(scatterArea.bounds.center, scatterArea.bounds.size);
    }
}
