using UnityEngine;

public class WorkGear : MonoBehaviour
{
    [SerializeField] private Collider gearCollider;
    [SerializeField] private WorkGearRequirements requirements;

    private void OnTriggerEnter(Collider other)
    {
        if (other.Equals(gearCollider))
        {
            requirements.PickUp(GetComponent<CustomInteraction>());
        }
    }
}
