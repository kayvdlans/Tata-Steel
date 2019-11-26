using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Downstairs : MonoBehaviour
{
    GameObject player;
    public Vector3 teleLocate;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public void TeleportToNewVector()
    {
        CharacterController c = player.GetComponent<CharacterController>();
        c.enabled = false;
        player.transform.position = teleLocate;
        c.enabled = true;
    }
}
