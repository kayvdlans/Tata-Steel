using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDoors : MonoBehaviour
{
    [SerializeField] private OpenedDoors openedDoors = null;
    [SerializeField] private int index = 0;
    public int Index { get { return index; } }

    [SerializeField] private Vector3 teleportlocation;

    [SerializeField] private Material matClosed = null;
    [SerializeField] private Material matOpen = null;
    [SerializeField] private GameObject matObject = null;

    [SerializeField] private AudioClip openDoor = null;
    [SerializeField] AudioSource audioSource;

    public GameObject player;

    private Renderer mat = null;
    private bool opened = false;
    public bool Opened
    {
        get { return opened; }
        set
        {
            opened = value;
            UpdateMaterial();
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Opened = openedDoors.IsAdded(index);
        StartCoroutine(CheckIfDoorOpened());
    }

    private void Update()
    {
        Opened = openedDoors.IsAdded(index);
    }

    private IEnumerator CheckIfDoorOpened()
    {
        while (!Opened)
        {
            yield return new WaitForSeconds(0.5f);
            Opened = openedDoors.IsAdded(index);
        }
    }

    private void UpdateMaterial()
    {
        if (mat == null)
            mat = matObject.GetComponent<Renderer>();

        mat.material = opened ? matOpen : matClosed;
    }



    public void TeleportToNewVector()
    {
        if (opened)
        {
            CharacterController c = player.GetComponent<CharacterController>();
            audioSource.PlayOneShot(openDoor, 0.7F);
            c.enabled = false;
            player.transform.position = teleportlocation;
            c.enabled = true;
        }
        else
        {
            //do something else?
        }
    }
}
