using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportGateManager : MonoBehaviour
{
    private GateController currentGate;
    public LightManager lightManager;
    public GameObject Player;
    [SerializeField] private int currentAreaRef = -1;

    void Start()
    {
        currentGate = null;
        if (lightManager != null)
        {
            currentAreaRef = lightManager.defaultAreaID;
        }
    }
    public void SetCurrentGate(GateController gate)
    {
        currentGate = gate;
    }

    public void RemoveGate()
    {
        currentGate = null;
    }

    void Update()
    {
        if (currentGate == null)
            return;

        // Ensure we have a player reference
        if (Player == null)
        {
            Player = GameObject.FindWithTag("Player");
            if (Player == null)
            {
                return;
            }
        }

        // Use the new input system if available, otherwise fallback to Input.GetKeyDown
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame || Keyboard.current.numpad1Key.wasPressedThisFrame) TryTeleport(0);
            if (Keyboard.current.digit2Key.wasPressedThisFrame || Keyboard.current.numpad2Key.wasPressedThisFrame) TryTeleport(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame || Keyboard.current.numpad3Key.wasPressedThisFrame) TryTeleport(2);
            if (Keyboard.current.digit4Key.wasPressedThisFrame || Keyboard.current.numpad4Key.wasPressedThisFrame) TryTeleport(3);
            if (Keyboard.current.digit5Key.wasPressedThisFrame || Keyboard.current.numpad5Key.wasPressedThisFrame) TryTeleport(4);
            if (Keyboard.current.digit6Key.wasPressedThisFrame || Keyboard.current.numpad6Key.wasPressedThisFrame) TryTeleport(5);
            if (Keyboard.current.digit7Key.wasPressedThisFrame || Keyboard.current.numpad7Key.wasPressedThisFrame) TryTeleport(6);
            if (Keyboard.current.digit8Key.wasPressedThisFrame || Keyboard.current.numpad8Key.wasPressedThisFrame) TryTeleport(7);
            if (Keyboard.current.digit9Key.wasPressedThisFrame || Keyboard.current.numpad9Key.wasPressedThisFrame) TryTeleport(8);
        }
        else
        {
            // Legacy Input fallback (supports main keyboard and numpad)
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) TryTeleport(0);
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) TryTeleport(1);
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) TryTeleport(2);
            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) TryTeleport(3);
            if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) TryTeleport(4);
            if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) TryTeleport(5);
            if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)) TryTeleport(6);
            if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)) TryTeleport(7);
            if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9)) TryTeleport(8);
        }
    }

    private void TryTeleport(int index)
    {
        if (currentGate == null)
        {
            return;
        }
        if (Player == null)
        {
            return;
        }
        if (lightManager == null)
        {
            Debug.LogWarning("TeleportGateManager: LightManager is not assigned.");
            return;
        }

        // Pre-validate gateindex to provide clearer warnings
        if (!HasExitGateIndex(index))
        {
            return;
        }

        int destinationAreaRef = currentGate.GetAreaRefNumber(index);
        if (destinationAreaRef < 0)
        {
            Debug.LogWarning($"[{currentGate.name}] Teleport failed: destination area is invalid at exit index {index}.");
            return;
        }

        currentGate.Teleport(Player.transform, index);

        if (currentAreaRef >= 0 && currentAreaRef != destinationAreaRef)
        {
            lightManager.SetAreaLightsActive(currentAreaRef, false);
        }

        lightManager.SetAreaLightsActive(destinationAreaRef, true);
        currentAreaRef = destinationAreaRef;
    }

    private bool HasExitGateIndex(int index)
    {
        if (currentGate == null)
            return false;

        if (currentGate.exitGates != null && currentGate.exitGates.Length > 0)
        {
            // If index is out of range, treat it as 0 (fall back to first exit) — Teleport will do the same.
            int checkIndex = (index < 0 || index >= currentGate.exitGates.Length) ? 0 : index;
            return currentGate.exitGates[checkIndex] != null;
        }

        // Fallback to single exitGate: accept any index and Teleport will use the single exitGate.
        if (currentGate.exitGate != null)
            return true;

        return false;
    }
}
