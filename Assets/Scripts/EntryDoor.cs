using System.Collections;
using UnityEngine;
using Unity.Cinemachine;    
public class EntryDoor : MonoBehaviour, IInteracable
{
    private TeleportGateManager gateManager;
    private bool error = false;
    private void Awake()
    {
        gateManager = FindFirstObjectByType<TeleportGateManager>();
        if (gateManager == null)
        {
            MessageBox.Show("Khong tim thay TeleportGateManager trong scene.");
            error = true;
        }
    }
    public void Interact()
    {
        if (error) return;
        gateManager.TryTeleport(0);
    }

    public void Highlight(bool highlight)
    {
        
    }
}
    
