using System.Collections;
using System;
using UnityEngine;

public class HapticFeedback : MonoBehaviour
{
    public enum FeedbackFrequency
    {
        Low,
        Medium, 
        High
    }

    [Serializable]
    private class Frequency
    {
        [SerializeField] [Range(160, 500)] private float low = 160;
        [SerializeField] [Range(160, 500)] private float medium = 330;
        [SerializeField] [Range(160, 500)] private float high = 500;

        public float Low { get { return low; } }
        public float Medium { get { return medium; } }
        public float High { get { return high; } }
    }

    [SerializeField] private Frequency frequency;

    [SerializeField] [Range(0, 1)] private float amplitude = 1;

    [SerializeField] private bool fadeOutFeedback;
    [SerializeField] private float fadeTime;

    //Set the controller actually
    public OVRInput.Controller Controller { get; private set; } = OVRInput.Controller.None;

    public void SetController()
    {
        Controller = OVRInput.GetActiveController();
    }

    public void AddFeedback(int strength)
    {
        float feedbackFrequency = MathHelper.NormalizeValueBetweenBounds(strength == (int)FeedbackFrequency.Low 
            ? this.frequency.Low : strength == (int)FeedbackFrequency.Medium 
            ? this.frequency.Medium : this.frequency.High, new Vector2(160, 500));

        OVRInput.SetControllerVibration(feedbackFrequency, amplitude, Controller);

        if (fadeOutFeedback)
            StartCoroutine(StartVibrationFade(fadeTime, feedbackFrequency, Controller));
    }

    private IEnumerator StartVibrationFade(float time, float frequency, OVRInput.Controller controller)
    {
        StopAllCoroutines();

        float currentAmp = amplitude;

        while (currentAmp > 0)
        {
            yield return new WaitForEndOfFrame();
            OVRInput.SetControllerVibration(frequency, currentAmp -= (Time.deltaTime / time), controller);
        }
    }
}
