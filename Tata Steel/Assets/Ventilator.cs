using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ventilator : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject ventilator;
    public GameObject voedingsPomp;
    public GameObject InternePomp;
    public void ButtonPressVentilator()
    {
        ventilator.SetActive(!ventilator.activeSelf);
    }

    public void ButtonPressVoedings()
    {
        voedingsPomp.SetActive(!voedingsPomp.activeSelf);
    }

    public void ButtonPressInternePomp()
    {
        InternePomp.SetActive(!InternePomp.activeSelf);
    }
}
