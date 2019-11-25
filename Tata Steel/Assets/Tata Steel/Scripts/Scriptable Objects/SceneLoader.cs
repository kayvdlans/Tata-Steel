using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneLoader", menuName = "ScriptableObjects/SceneLoader")]
public class SceneLoader : ScriptableObject
{
    [SerializeField]
    private float transitionTime;

    private OVRScreenFade fadeEvent;

    public void LoadScene(RoomSettings room)
    {
        fadeEvent = Camera.main.GetComponent<OVRScreenFade>();

        if (!fadeEvent)
        {
            SceneManager.LoadScene(room.SceneName, LoadSceneMode.Single);
            Debug.LogWarning("Failed to find FadeEvent, so skipping it");
            return;
        }

        fadeEvent.fadeTime = transitionTime;
        fadeEvent.FadeOut();
        fadeEvent.StartCoroutine(Transition(room.SceneName));
    }

    public void LoadScene(string sceneName)
    {
        fadeEvent = Camera.main.GetComponent<OVRScreenFade>();

        if (!fadeEvent)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            Debug.LogWarning("Failed to find FadeEvent, so skipping it");
            return;
        }

        fadeEvent.fadeTime = transitionTime;
        fadeEvent.FadeOut();
        fadeEvent.StartCoroutine(Transition(sceneName));
    }

    private IEnumerator Transition(string sceneName)
    {
        while (fadeEvent.currentAlpha < 1)
        {
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
