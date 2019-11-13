using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private OpenedDoors openedDoors = null;
    [SerializeField] private RoomSettings room;
    public RoomSettings Room { get { return room; } }

    [SerializeField] private Material matClosed = null;
    [SerializeField] private Material matOpen = null;
    [SerializeField] private GameObject matObject = null;

    [SerializeField] private AudioClip openDoor = null;
    [SerializeField] AudioSource audioSource;

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
        Opened = openedDoors.IsAdded(room.RoomID);
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        Opened = openedDoors.IsAdded(room.RoomID);
    }

    private void UpdateMaterial()
    {
        if (mat == null)
            mat = matObject.GetComponent<Renderer>();

        mat.material = opened ? matOpen : matClosed;
    }

    public void LoadSceneOnDoorInteraction(SceneLoader sceneLoader)
    {
        if (opened)
        {
            audioSource.PlayOneShot(openDoor, 0.7F);
            sceneLoader.LoadScene(room);
        }
        else
        {
            Debug.Log("Can't open door to room: " + room.SceneName + ", since the door hasn't been activated yet.");
            //do something else?
        }
    }
}
