using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UserTestTimer : MonoBehaviour
{
    [SerializeField] private Text timeText;
    [SerializeField][Range(1, 60)] private float timeUntilScatter;

    public UnityAction OnTimerFinished { get; set; }

    private ObjectInteractionState interactionState;

    private void Start()
    {
        interactionState = FindObjectOfType<ObjectInteractionState>();
        interactionState.OnUpdateState += CheckState;

        timeText.gameObject.SetActive(false);
    }

    private void CheckState(ObjectInteractionState.State state)
    {
        if (state.Equals(ObjectInteractionState.State.Scatter))
        {
            StopAllCoroutines();
            StartCoroutine(WaitForGrabbableState());
        }
    }

    private IEnumerator WaitForGrabbableState()
    {
        timeText.gameObject.SetActive(true);

        float timeLeft = timeUntilScatter;

        while (timeLeft > 0) 
        {
            timeLeft -= Time.deltaTime;
            timeText.text = ((int) timeLeft).ToString();

            yield return new WaitForEndOfFrame();
        }

        timeText.gameObject.SetActive(false);

        OnTimerFinished?.Invoke();
    }
}
