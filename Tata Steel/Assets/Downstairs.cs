using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Downstairs : MonoBehaviour
{
    GameObject player;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public void TeleportToNewVector()
    {
        CharacterController c = player.GetComponent<CharacterController>();
        c.enabled = false;
        player.transform.position = new Vector3(4.278f, 0.435f, 0.36f);
        c.enabled = true;
    }
}
