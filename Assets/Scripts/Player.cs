using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour,ILivingEntity
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Input")]
    [SerializeField] private string actionMapName = "Player";
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string skipActionName = "SkipText";

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private static readonly string ANIM_PARAM_IS_MOVING = "IsMoving";
    private static readonly string ANIM_PARAM_MOVE_X = "MoveX";
    private static readonly string ANIM_PARAM_MOVE_Y = "MoveY";

    // 3D Rigidbody
    private Rigidbody rb;
    private InputAction moveAction;
    private InputAction skipAction;

    private Vector2 moveInput;
    private bool isMoving;
    private bool moveable = true;

    // Lưu hướng animator cuối cùng (mặc định là nhìn phải) 
    private Vector2 lastAnimDirection = new Vector2(1, 0);
    private bool m_isAlive = true;
    public bool IsAlive { get { return m_isAlive; } set { m_isAlive = value; } }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (animator == null)
            animator = GetComponent<Animator>();

        // Prefer retrieving configured actions from VDGlobal if available
        if (VDGlobal.Instance != null)
        {
            moveAction = VDGlobal.Instance.MoveAction ?? VDGlobal.Instance.InputActions?.FindActionMap(actionMapName)?.FindAction(moveActionName);
            skipAction = VDGlobal.Instance.SkipAction ?? VDGlobal.Instance.InputActions?.FindActionMap(actionMapName)?.FindAction(skipActionName);
        }
        else
        {
            var inputActions = VDGlobal.Instance?.InputActions;
            if (inputActions == null)
            {
                Debug.LogWarning("Player: InputActionAsset not available from VDGlobal.");
            }
            else
            {
                moveAction = inputActions.FindActionMap(actionMapName)?.FindAction(moveActionName);
                skipAction = inputActions.FindActionMap(actionMapName)?.FindAction(skipActionName);
            }
        }
    }

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.Enable();
            moveAction.performed += OnMovePerformed;
            moveAction.canceled += OnMoveCanceled;
        }

        if (skipAction != null)
        {
            skipAction.Enable();
            skipAction.performed += OnSkipPerformed;
        }
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.performed -= OnMovePerformed;
            moveAction.canceled -= OnMoveCanceled;
            moveAction.Disable();
        }

        if (skipAction != null)
        {
            skipAction.performed -= OnSkipPerformed;
            skipAction.Disable();
        }
    }

    private void OnSkipPerformed(InputAction.CallbackContext context)
    {
        // invoke dialog skip if available
        DialogBox.Instance?.Skip();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    // Enable or disable player movement input/action.
    // When disabled, movement input is cleared and animation updated.
    public void setMoveable(bool enabled)
    {
        if (moveable == enabled)
            return;

        moveable = enabled;

        if (moveAction == null)
            return;

        if (moveable)
        {
            moveAction.Enable();
        }
        else
        {
            moveAction.Disable();
            moveInput = Vector2.zero;
            isMoving = false;
            UpdateAnimation();
        }
    }

    private void Update()
    {
        isMoving = moveInput.sqrMagnitude > 0.001f;
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (rb == null) return;

        // Build horizontal movement vector and normalize only the horizontal axes.
        Vector3 horizontal = new Vector3(moveInput.x, 0f, moveInput.y);
        if (horizontal.sqrMagnitude > 1f)
            horizontal.Normalize();

        // Apply horizontal speed but preserve current vertical velocity (gravity/jumps).
        Vector3 velocity = new Vector3(
            horizontal.x * moveSpeed,
            rb.linearVelocity.y,
            horizontal.z * moveSpeed
        );

        rb.linearVelocity = velocity;
    }
    public void Die()
    {
        m_isAlive = false;
        setMoveable(false);
    }
    private void UpdateAnimation()
    {
        if (animator == null) return;

        animator.SetBool(ANIM_PARAM_IS_MOVING, isMoving);

        if (isMoving)
        {
            // Logic 4 hướng: Chọn trục có giá trị lớn hơn
            if (Mathf.Abs(moveInput.x) >= Mathf.Abs(moveInput.y))
            {
                // Ưu tiên trục X (Trái/Phải)
                lastAnimDirection = new Vector2(Mathf.Sign(moveInput.x), 0);
            }
            else
            {
                // Ưu tiên trục Y (Lên/Xuống)
                lastAnimDirection = new Vector2(0, Mathf.Sign(moveInput.y));
            }
        }

        // Gửi vector đã chuẩn hóa (1,0), (-1,0), (0,1), hoặc (0,-1) vào Animator
        animator.SetFloat(ANIM_PARAM_MOVE_X, lastAnimDirection.x);
        animator.SetFloat(ANIM_PARAM_MOVE_Y, lastAnimDirection.y);
    }
}
