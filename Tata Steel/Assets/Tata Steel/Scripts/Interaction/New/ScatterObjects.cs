using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScatterObjects : MonoBehaviour
{
    [Header("Scatter Preparation")]
    [SerializeField] private Collider scatterArea;
    [SerializeField] private float timeBetweenScattering;
    [SerializeField] private float scatterOffsetY;
    [Header("Object Manipulation")]
    [SerializeField] [Range(0.01f, 1f)] private float objectMoveTreshold;
    [SerializeField] [Range(0.01f, 1f)] private float objectMoveSpeed;

    private List<GameObject> objects = new List<GameObject>();
    private UserTestTimer timer;
    private ObjectInteractionState interactionState;

    private void Start()
    {
        objects.AddRange(GameObject.FindGameObjectsWithTag("UserTestObject"));
        interactionState = FindObjectOfType<ObjectInteractionState>();
        timer = FindObjectOfType<UserTestTimer>();
        timer.OnTimerFinished += Scatter;
    }

    private void Scatter()
    {
        StopAllCoroutines();
        StartCoroutine(ScatterObject(timeBetweenScattering));
    }

    private void ShuffleObjects(List<GameObject> objects)
    {
        System.Random random = new System.Random();

        int n = objects.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            GameObject value = objects[k];
            objects[k] = objects[n];
            objects[n] = value;
        }
    }

    private IEnumerator ScatterObject(float time)
    {
        ShuffleObjects(objects);

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
        r.isKinematic = true;

        float originalDistance = (destination - ob.transform.position).magnitude;
        float distance;
        while ((distance = (destination - ob.transform.position).magnitude) > objectMoveTreshold)
        {
            //ratio is used to smooth the move speed based on the distance to the destination
            float ratio = (distance + objectMoveTreshold) / originalDistance;

            ob.transform.position = Vector3.MoveTowards(ob.transform.position, destination, speed * ratio);

            yield return new WaitForEndOfFrame();
        }

        r.useGravity = true;
        r.isKinematic = false;
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
