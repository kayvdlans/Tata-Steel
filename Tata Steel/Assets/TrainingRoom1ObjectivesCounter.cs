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
        flask.tag = "Flask";
        stoomafsluiters.tag = "Stoomafsluiters";
        aftapsluiter.tag = "Aftapsluiter";
    }

    void Update()
    {

    }
}

