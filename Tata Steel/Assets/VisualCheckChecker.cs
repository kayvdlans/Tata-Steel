using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualCheckChecker : MonoBehaviour
{
    TrainingRoom1ObjectivesCounter trainingScript;
    private Transform _selection;
    [SerializeField] private Material defaultMaterial;
    public string ToLookAt;
    public string LookingAt;

    // Update is called once per frame
    void Update()
    {
        /*if (_selection != null)
          {
            var selectionRenderer = _selection.GetComponent<Renderer>();
            selectionRenderer.material = defaultMaterial;
            _selection = null;
        }*/

        Ray ray = new Ray(Camera.main.transform.position,Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, LayerMask.NameToLayer(ToLookAt)))
        {
            if (hit.collider.tag == ToLookAt)
            {
                LookingAt = hit.collider.name;
                Debug.Log(LookingAt);
            }
        }

    }
}
