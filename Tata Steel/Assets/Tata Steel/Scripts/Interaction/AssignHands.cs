using System.Collections;
using UnityEngine;

public class AssignHands : MonoBehaviour
{
    [SerializeField] private Transform localAvatar;

    private Transform leftHand = null;
    private Transform rightHand = null;

    private void Start()
    {
        StartCoroutine(Initialize(0.5f));
    }

    private IEnumerator Initialize(float waitTime)
    {
        while (!leftHand || !rightHand)
        {
            yield return new WaitForSeconds(waitTime);

            leftHand = localAvatar.Find("hand_left");
            rightHand = localAvatar.Find("hand_right");
        }

        leftHand.gameObject.AddComponent<Hand>();
        rightHand.gameObject.AddComponent<Hand>();

        Destroy(this);
    }
}
