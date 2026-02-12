using UnityEngine;
using UnityEngine.InputSystem;

public class VDGlobal : MonoBehaviour
{
    public static VDGlobal Instance { get; private set; }

    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string actionMapName = "Player";
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string interactActionName = "Interact";
    [SerializeField] private string skipActionName = "SkipText";
    [SerializeField] private Transform playerTranform;
    [SerializeField] private PlayerInteractor playerInteractor;

    public InputActionAsset InputActions=> inputActions;
    public InputAction MoveAction => moveAction;
    public InputAction InteractAction => interactAction;
    public InputAction SkipAction => skipAction;

    public Transform PlayerTranform => playerTranform;
    public PlayerInteractor PlayerInteractor =>playerInteractor;

    private InputActionMap actionMap;
    private InputAction moveAction;
    private InputAction interactAction;
    private InputAction skipAction;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (inputActions != null)
        {
            actionMap = inputActions.FindActionMap(actionMapName);
            if (actionMap == null)
            {
            }
            else
            {
                moveAction = actionMap.FindAction(moveActionName);
                interactAction = actionMap.FindAction(interactActionName);
                skipAction = actionMap.FindAction(skipActionName);
            }
        }
        else
        {
        }
    }

    // Enable the Move action (will enable its action map if not already enabled)
    public void EnableMoveAction()
    {
        if (actionMap != null && !actionMap.enabled)
            actionMap.Enable();

        if (moveAction != null && !moveAction.enabled)
            moveAction.Enable();
    }

    // Enable the Interact action (will enable its action map if not already enabled)
    public void EnableInteractAction()
    {
        if (actionMap != null && !actionMap.enabled)
            actionMap.Enable();

        if (interactAction != null && !interactAction.enabled)
            interactAction.Enable();
    }

    // Disable the Move action (will disable its action map if no other actions are needed)
    public void DisableMoveAction()
    {
        if (moveAction != null && moveAction.enabled)
            moveAction.Disable();

    }

    // Disable the Interact action
    public void DisableInteractAction()
    {
        if (interactAction != null && interactAction.enabled)
            interactAction.Disable();
    }

    // Optional helper to check current state
    public bool IsMoveActionEnabled => moveAction != null && moveAction.enabled;
    public bool IsInteractActionEnabled => interactAction != null && interactAction.enabled;
    public bool IsSkipActionEnabled => skipAction != null && skipAction.enabled;

  
}
