using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;
public class Player : MonoBehaviour, ILivingEntity, IDataPersistance, IOnSceneReady
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }

    [Header("Input")]
    [SerializeField] private string actionMapName = "Player";
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string skipActionName = "SkipText";

    [Header("Animation")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PlayerAnimator playerAnimatorComponent;
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
        Debug.Log("Player Awake on object: " + gameObject.name  );
        rb = GetComponent<Rigidbody>();     
         if (VDGlobal.Instance != null)
        {
            moveAction = VDGlobal.Instance.MoveAction ?? VDGlobal.Instance.InputActions?.FindActionMap(actionMapName)?.FindAction(moveActionName);
            skipAction = VDGlobal.Instance.SkipAction ?? VDGlobal.Instance.InputActions?.FindActionMap(actionMapName)?.FindAction(skipActionName);
        }
        else
        {
           Debug.LogError("VDGlobal instance not found. Ensure VDGlobal is initialized before Player.");
        }
        if (playerAnimatorComponent == null)
            playerAnimatorComponent = GetComponentInChildren<PlayerAnimator>();
            if (playerAnimatorComponent != null)
            {
                Debug.Log("Dang ky su kien hoi sinh");
               playerAnimatorComponent.OnDeathAnimationComplete +=Revive;
            }
   
    }
    public void LoadData(GameData gameData)
{
    if (gameData == null) return;

    Vector3 oldPos = transform.position;
    Vector3 savedPos = gameData.playerPosition.ToVector3();
    Vector3 finalPosition = savedPos;
    SetPosition(finalPosition);
    CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
    if (cam != null)
    {
        // Delta là khoảng cách từ vị trí cũ tới vị trí mới
        Vector3 delta = finalPosition - oldPos;
        cam.OnTargetObjectWarped(transform, delta);
    }
    }


    public void SaveData(ref GameData gameData)
    {
        if (gameData == null) return;
        Debug.Log("Saving player position: " + transform.position);
        gameData.playerPosition = new SerializableVector3(transform.position);
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
        Debug.Log("Moving");
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    // Enable or disable player movement input/action.
    // When disabled, movement input is cleared and animation updated.
    public void SetMoveable(bool enabled)
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
        Vector3 horizontal = new(moveInput.x, 0f, moveInput.y);
        if (horizontal.sqrMagnitude > 1f)
            horizontal.Normalize();

        // Apply horizontal speed but preserve current vertical velocity (gravity/jumps).
        Vector3 velocity = new(
            horizontal.x * moveSpeed,
            rb.linearVelocity.y,
            horizontal.z * moveSpeed
        );

        rb.linearVelocity = velocity;
    }
    public void Die()
    {
        m_isAlive = false;
        SetMoveable(false);
        if (playerAnimatorComponent != null)
        {
            playerAnimatorComponent.DoDie();
        }
     }
    public void Revive()
    {
        Debug.Log("Hoi sinh nhan vat");
        m_isAlive = true;
        SetMoveable(true);
        if (playerAnimatorComponent != null)
        {
           playerAnimatorComponent.DoRevive();
        }
        // SetPosition(DataPersistanceManager.Instance.gameData.playerPosition.ToVector3());
        MainCanvas.Instance.SetGlobalSaturation(0);
        MainCanvas.Instance.ScreenTransitionController.PlayTransition(TransitionType.VerticalShutter, TransitionDirection.Out, () =>
        {
            GameManager.Instance.PlayerCamera.ZoomOut();
        }
         );
       
    }
    public void SetPosition(Vector3 targetPos)
    {
        int groundLayerMask = LayerMask.GetMask("Ground"); 
    if (Physics.Raycast(targetPos + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 5f, groundLayerMask))
    {
        targetPos = hit.point;
    }

    // 2. Dịch chuyển Player
    if (rb != null)
    {
        
        // Gán vị trí cho cả 2 để đồng bộ ngay lập tức
        transform.position = targetPos;
        rb.position = targetPos;
        
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Đảm bảo Unity cập nhật vị trí vật lý ngay lập tức trước khi bật lại Kinematic
        Physics.SyncTransforms(); 
    }
    else
    {
        transform.position = targetPos;
    }
    }
     private void OnDestroy()
    {
        if (playerAnimatorComponent != null)
        {
            playerAnimatorComponent.OnDeathAnimationComplete -= Revive;
        }
    }
    private void UpdateAnimation()
    {
        if (playerAnimator == null) return;

        playerAnimator.SetBool(ANIM_PARAM_IS_MOVING, isMoving);

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
        playerAnimator.SetFloat(ANIM_PARAM_MOVE_X, lastAnimDirection.x);
        playerAnimator.SetFloat(ANIM_PARAM_MOVE_Y, lastAnimDirection.y);
    }

    public IEnumerator OnSceneReady()
    {
        yield return new WaitForFixedUpdate(); 
    }
}
