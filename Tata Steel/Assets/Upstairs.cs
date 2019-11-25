using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upstairs : MonoBehaviour
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
        player.transform.position = new Vector3(1.51f, 1.83f, 0.36f);
            c.enabled = true;
    }
}
