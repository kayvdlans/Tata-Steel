using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateDeactivateObject : MonoBehaviour
{
    [SerializeField]
    private GameObject go;

    [SerializeField]
    private float activationCooldown = 1f;

    private bool canActivate = true;

    public void ActivateDeactivate()
    {
        if (canActivate)
        {
            go.SetActive(!go.activeSelf);
            StartCoroutine(ResetActivationMechanism());
        }
    } 

    private IEnumerator ResetActivationMechanism()
    {
        canActivate = false;

        yield return new WaitForSeconds(activationCooldown);

        canActivate = true;
    }
}
