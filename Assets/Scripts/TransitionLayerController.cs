using UnityEngine;
using DG.Tweening;

public class TransitionLayerController : MonoBehaviour
{
    [SerializeField] private RectTransform maskRectTransform;
    [SerializeField] private float duration = 1f;
    [SerializeField] private Ease ease = Ease.OutCubic;

    private Tween currentTween;

    private void Awake()
    {
        if (maskRectTransform == null)
        {
            Debug.LogError("Mask RectTransform is not assigned.");
            enabled = false;
            return;
        }
    }

    /// <summary>
    /// Mở màn chập (scale X từ 0 -> 1)
    /// </summary>
    public void OpenCurtain()
    {
        KillCurrentTween();

        maskRectTransform.localScale = new Vector3(0f, 1f, 1f);

        currentTween = maskRectTransform
            .DOScaleX(1f, duration)
            .SetEase(ease);
    }

    /// <summary>
    /// Đóng màn chập (scale X từ 1 -> 0)
    /// </summary>
    public void CloseCurtain()
    {
        KillCurrentTween();

        maskRectTransform.localScale = new Vector3(1f, 1f, 1f);

        currentTween = maskRectTransform
            .DOScaleX(0f, duration)
            .SetEase(ease);
    }

    /// <summary>
    /// Mở màn chập với callback khi hoàn tất
    /// </summary>
    public void OpenCurtain(System.Action onComplete)
    {
        KillCurrentTween();

        maskRectTransform.localScale = new Vector3(0f, 1f, 1f);

        currentTween = maskRectTransform
            .DOScaleX(1f, duration)
            .SetEase(ease)
            .OnComplete(() => onComplete?.Invoke());
    }

    private void KillCurrentTween()
    {
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }
    }
}
