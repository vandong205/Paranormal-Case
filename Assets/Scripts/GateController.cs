using UnityEngine;

public class GateController : MonoBehaviour
{
    [Tooltip("(Deprecated) Single exit gate. Kept for compatibility.")]
    public Transform exitGate;

    [Tooltip("Multiple exit gates. Use Teleport(obj, index) to choose one.")]
    public Transform[] exitGates;

    [Tooltip("Objects to enable when the player enters the gate trigger; they will be disabled on exit.")]
    public GameObject[] triggerObjects;

    public TeleportGateManager Manager;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player đã vào vùng Teleport " + this.name);
            if(Manager != null){
                Manager.SetCurrentGate(this);
            }
            // Enable any associated trigger objects
            if (triggerObjects != null)
            {
                foreach (var go in triggerObjects)
                {
                    if (go != null)
                        go.SetActive(true);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player đã ra khỏi vùng Teleport" + this.name);
            if(Manager != null){
                Manager.RemoveGate();
            }
            // Disable any associated trigger objects
            if (triggerObjects != null)
            {
                foreach (var go in triggerObjects)
                {
                    if (go != null)
                        go.SetActive(false);
                }
            }
        }
    }
    public void Teleport(Transform obj, int index)
    {
        if (obj == null)
        {
            Debug.LogWarning($"[{name}] Teleport failed: object to teleport is null.");
            return;
        }
        // Prefer configured exitGates array when present
        if (exitGates != null && exitGates.Length > 0)
        {
            int useIndex = index;
            if (index < 0 || index >= exitGates.Length)
            {
                Debug.LogWarning($"[{name}] Teleport: index {index} out of range, falling back to index 0.");
                useIndex = 0;
            }

            var target = exitGates[useIndex];
            if (target == null)
            {
                Debug.LogWarning($"[{name}] Teleport failed: exitGates[{useIndex}] is null.");
                return;
            }

            obj.position = target.position;
            return;
        }

        // Fallback to single exitGate for compatibility
        if (exitGate != null)
        {
            if (index != 0)
            {
                Debug.Log($"[{name}] Teleport: requested index {index} but only single exitGate configured, using single exitGate.");
            }

            obj.position = exitGate.position;
            return;
        }

        Debug.LogWarning($"[{name}] Teleport failed: no exit gates configured (exitGate or exitGates required).");
    }

    // Backwards-compatible overload: teleport to the first (or single) gate
    public void Teleport(Transform obj)
    {
        Teleport(obj, 0);
    }
    public int GetAreaRefNumber(int exitGateIndex = -1)
    {
        if (exitGates == null || exitGates.Length == 0)
        {
            if (exitGate != null && exitGate.TryGetComponent<ExitGateData>(out var singleGateData))
            {
                return singleGateData.areaRefNumber;
            }

            return -1;
        }

        int useIndex = exitGateIndex;
        if (exitGateIndex < 0 || exitGateIndex >= exitGates.Length)
        {
            useIndex = 0;
        }

        if (exitGates[useIndex] != null && exitGates[useIndex].TryGetComponent<ExitGateData>(out var gateData))
        {
            return gateData.areaRefNumber;
        }

        return -1;
    }

}
