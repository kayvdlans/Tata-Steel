using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonInteraction : MonoBehaviour
{
    public Material materialToChange;
    public GameObject button;
    // Start is called before the first frame update

    public UnityEvent onInteract;
 
    private void OnTriggerEnter(Collider other)
    {
        materialToChange.color = Color.red;
        button.transform.localPosition = new Vector3(-0.00001f, 0, 0);
        onInteract.Invoke();
    }
    private void OnTriggerExit(Collider other)
    {
        materialToChange.color = Color.black;
        button.transform.localPosition = new Vector3(0, 0, 0);
    }
}
