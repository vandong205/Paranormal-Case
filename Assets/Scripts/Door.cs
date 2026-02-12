using UnityEngine;

public class Door : MonoBehaviour, IInteracable
{

    [SerializeField] Outline outliner;
    [SerializeField] private Animator animator;
    [SerializeField] private bool opened = false;
   

    private static readonly string ANIM_PARAM_OPENED = "Opened";

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator != null)
            animator.SetBool(ANIM_PARAM_OPENED, opened);
        // Ensure the interaction icon / outline is hidden by default
        if (outliner == null)
            outliner = GetComponentInChildren<Outline>();

        if (outliner != null)
            outliner.enabled = false;
    }

    // Toggle door state when interacted
    public void Interact()
    {
        Debug.Log("Interacted with door: " + gameObject.name);
        DialogBox.Instance.PushText(opened ? "Closing the door." : "Opening the door.");
        Toggle();
    }

    // Toggle helper
    public void Toggle()
    {
        opened = !opened;
        if (animator != null)
            animator.SetBool(ANIM_PARAM_OPENED, opened);
    }

    // Show or hide outline/highlight
    public void Highlight(bool highlight)
    {
        if (outliner == null)
            outliner = GetComponentInChildren<Outline>();

        if (outliner != null)
            outliner.enabled = highlight;
    }

}
