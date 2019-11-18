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
        if (_selection != null)
          {
            var selectionRenderer = _selection.GetComponent<Renderer>();
            selectionRenderer.material = defaultMaterial;
            _selection = null;
        }


        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.height / 2, Screen.width / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == ToLookAt)
            {
                Debug.Log(hit.collider.name);
                LookingAt = hit.collider.name;
            }
        }

    }
}
