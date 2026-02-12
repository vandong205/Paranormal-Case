using System.Collections;
using UnityEngine;

public class EntryDoor : MonoBehaviour, IInteracable
{
    [Header("Interact")]
    [SerializeField] private Outline outliner;
    [Header("Teleport")]
    [SerializeField] private Transform targetPoint;

    [Header("Settings")]
    [SerializeField] private float teleportDelay = 0.05f;

    private bool isProcessing = false;
    private void Awake()
    {
        if(outliner!=null)
            outliner.enabled = false;
    }

    public void Interact()
    {
        if (isProcessing) return;

        StartCoroutine(TeleportSequence());
    }

    public void Highlight(bool highlight)
    {
        if(outliner == null)
            outliner = GetComponentInChildren<Outline>();

        if (outliner != null)
            outliner.enabled = highlight;
    }

    // =========================
    // Core logic
    // =========================

    private IEnumerator TeleportSequence()
    {
        isProcessing = true;

        // 1. Khóa input
        VDGlobal.Instance.DisableMoveAction();
        VDGlobal.Instance.DisableInteractAction();

        VDGlobal.Instance.PlayerInteractor.ForceClearInteractable();
        // 2. Fade out
        yield return FadeScreen.Instance.FadeOut();

        // 3. Delay nhỏ chống glitch
        if (teleportDelay > 0f)
            yield return new WaitForSeconds(teleportDelay);

        // 4. Teleport
        TeleportPlayer();

        // 5. Fade in
        yield return FadeScreen.Instance.FadeIn();

        // 6. Mở lại input
        VDGlobal.Instance.EnableMoveAction();
        VDGlobal.Instance.EnableInteractAction();

        isProcessing = false;
    }

    private void TeleportPlayer()
    {
        if (VDGlobal.Instance.PlayerTranform == null || targetPoint == null)
        {
            return;
        }

        Transform playerTransform = VDGlobal.Instance.PlayerTranform;

        CharacterController characterController =
            playerTransform.GetComponent<CharacterController>();

        if (characterController != null)
            characterController.enabled = false;

        Rigidbody rigidbody = playerTransform.GetComponent<Rigidbody>();
        if (rigidbody != null)
            rigidbody.linearVelocity = Vector3.zero;

        playerTransform.position = targetPoint.position;
        playerTransform.rotation = targetPoint.rotation;

        if (characterController != null)
            characterController.enabled = true;
    }
}
