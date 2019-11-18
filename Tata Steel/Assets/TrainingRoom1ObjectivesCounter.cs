using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingRoom1ObjectivesCounter : MonoBehaviour
{
    public GameObject flask;
    public GameObject stoomafsluiters;
    public GameObject aftapsluiter;
    // Update is called once per frame
    private void Start()
    {
        flask.tag = "LookAt";
        stoomafsluiters.tag = "LookAt";
        aftapsluiter.tag = "LookAt";
    }

    void Update()
    {

    }
}

