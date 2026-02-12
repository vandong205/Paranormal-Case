using UnityEngine;
using System.Collections;
public class EntryImmobleDoorCompose : MonoBehaviour
{
    [SerializeField] private ImmobileDoor immobileDoor;
    [SerializeField] private EntryDoor entryDoor;
    [SerializeField] private Collider triggerCollider;


    private void Awake()
    {
        if (entryDoor != null)
            entryDoor.enabled = false; // ban đầu không cho vào

        if (immobileDoor != null)
            immobileDoor.OnDoorUnlocked += HandleDoorUnlocked;
    }

    private void OnDestroy()
    {
        if (immobileDoor != null)
            immobileDoor.OnDoorUnlocked -= HandleDoorUnlocked;
    }

    private void HandleDoorUnlocked()
    {
        Debug.Log("Door unlocked");

        var interactor = VDGlobal.Instance.PlayerInteractor;

        // 1. Clear interactable cũ
        interactor.ForceClearInteractable();

        // 2. Disable cửa khóa
        if (immobileDoor != null)
            immobileDoor.enabled = false;

        // 3. Reset trigger vật lý
        if (triggerCollider != null)
            StartCoroutine(ResetTrigger(triggerCollider));

        // 4. Enable cửa mới
        if (entryDoor != null)
            entryDoor.enabled = true;
    }
    IEnumerator ResetTrigger(Collider col)
    {
        col.enabled = false;
        yield return new WaitForFixedUpdate();
        col.enabled = true;
    }

}
