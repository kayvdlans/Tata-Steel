using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCScreen : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ventilator;
    public GameObject voedingspomp;
    public GameObject internewaterkoeler;

    public bool venti = false;
    public bool voedi = false;
    public bool inter = false;


    public void ventilatorKnop()
    {
        venti = !venti;
    }

    public void voedingspompKnop()
    {
        voedi = !voedi;
    }

    public void internewaterkoelerKnop()
    {
        inter = !inter;
    }

}
