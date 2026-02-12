using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour, IInteractor
{
    [SerializeField] private string actionMapName = "Player";
    [SerializeField] private string interactActionName = "Interact";
    [SerializeField] private bool autoInteract = false;
    public bool AutoInteract => autoInteract;
    private InputActionMap actionMap;
    private InputAction interactAction;
    private IInteracable currentInteracable;

    private void OnEnable()
    {
        // Prefer VDGlobal's action object if available
        if (VDGlobal.Instance != null && VDGlobal.Instance.InteractAction != null)
        {
            interactAction = VDGlobal.Instance.InteractAction;
            interactAction.performed += OnInteractPerformed;
            if (!interactAction.enabled) interactAction.Enable();
            return;
        }

        var inputActions = VDGlobal.Instance?.InputActions;
        if (inputActions == null)
        {
            return;
        }

        actionMap = inputActions.FindActionMap(actionMapName);
        if (actionMap == null)
        {
            return;
        }

        actionMap.Enable();
        interactAction = actionMap.FindAction(interactActionName);
        if (interactAction != null)
        {
            interactAction.performed += OnInteractPerformed;
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.performed -= OnInteractPerformed;
            // If interactAction came from VDGlobal, don't disable actionMap here (VDGlobal manages it)
            if (VDGlobal.Instance == null || VDGlobal.Instance.InteractAction != interactAction)
            {
                if (interactAction.enabled) interactAction.Disable();
            }
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        var interactables = other.GetComponents<IInteracable>();
        if (interactables == null || interactables.Length == 0)
            return;

        foreach (var interactable in interactables)
        {
            if (interactable is MonoBehaviour mb && mb.enabled)
            {
                OnInteractableEnter(interactable);
                return; 
            }
        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<IInteracable>(out var interacable))
            return;

        OnInteractableExit(interacable);
    }
    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        if (currentInteracable == null) return;
        Debug.Log("PlayerInteractor: Interact performed with " + currentInteracable.ToString());
        currentInteracable.Interact();
    }
    // IInteractor implementation - called by interactable objects when player enters their trigger
    public void OnInteractableEnter(IInteracable interacable)
    {
        ClearCurrentInteractableData();
        Debug.Log("Enter interactable: " + interacable.ToString());
        if (interacable == null)
            return;

        // enable highlight when entering
        interacable.Highlight(true);

        if (autoInteract)
        {
            interacable.Interact();
            return;
        }

        currentInteracable = interacable;
    }

    // IInteractor implementation - called by interactable objects when player exits their trigger
    public void OnInteractableExit(IInteracable interacable)
    {
        if (interacable == null)
            return;
        Debug.Log("Exit interactable: " + interacable.ToString());
        // disable highlight when exiting
        interacable.Highlight(false);
        ClearCurrentInteractableData();
        
    
    }
    public void ClearCurrentInteractableData()
    {
        if(currentInteracable == null) return;
        currentInteracable.Highlight(false);
        currentInteracable = null;
    }
    public void ForceClearInteractable()
    {
        if (currentInteracable == null) return;

        currentInteracable.Highlight(false);
        currentInteracable = null;
    }
    public void TryBindInteractable(Collider trigger)
    {
        var interactables = trigger.GetComponents<IInteracable>();
        if (interactables == null || interactables.Length == 0)
            return;

        foreach (var interactable in interactables)
        {
            if (interactable is MonoBehaviour mb && mb.enabled)
            {
                OnInteractableEnter(interactable);
                return;
            }
        }
    }

}
