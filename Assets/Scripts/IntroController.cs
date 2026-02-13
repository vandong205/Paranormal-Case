using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using DG.Tweening;

public class IntroController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Image blackOverlayImage;
    public RectTransform videoRect;

    public float fadeDuration = 1.5f;

    [Header("Zoom Settings")]
    public float initialScale = 0.9f;     // Không full ngay đầu
    public float dipScale = 0.82f;        // Zoom nhỏ thêm chút
    public float finalScale = 1f;         // Full màn hình
    public float dipDuration = 0.6f;
    public float shootDuration = 1.2f;

    private bool isTransitioning = false;

    void Start()
    {
        videoPlayer.isLooping = false;

        // Clear render texture tránh frame cũ
        if (videoPlayer.targetTexture != null)
        {
            RenderTexture.active = videoPlayer.targetTexture;
            GL.Clear(true, true, Color.black);
            RenderTexture.active = null;
        }

        // Không full ngay frame đầu
        videoRect.localScale = Vector3.one * initialScale;

        SetOverlayAlpha(0f);

        videoPlayer.loopPointReached += OnVideoFinished;

        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        vp.Play();
        PlayZoomEffect();
    }

    void Update()
    {
        if (!isTransitioning && Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            isTransitioning = true;
            StartCoroutine(FadeAndLoadScene());
        }

        if (!isTransitioning && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            isTransitioning = true;
            StartCoroutine(FadeAndLoadScene());
        }
    }

    private void PlayZoomEffect()
    {
        Sequence seq = DOTween.Sequence();

        // 1️⃣ Zoom nhỏ thêm chút (ngược)
        seq.Append(
            videoRect.DOScale(dipScale, dipDuration)
            .SetEase(Ease.InQuad)
        );

        // 2️⃣ Shoot ra full
        seq.Append(
            videoRect.DOScale(finalScale, shootDuration)
            .SetEase(Ease.OutCubic)
        );
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (isTransitioning) return;

        isTransitioning = true;
        StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        float elapsedTime = 0f;
        Color overlayColor = blackOverlayImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            overlayColor.a = t;
            blackOverlayImage.color = overlayColor;
            yield return null;
        }

        overlayColor.a = 1f;
        blackOverlayImage.color = overlayColor;

        SceneManager.LoadScene("MainMenu");
    }

    private void SetOverlayAlpha(float alpha)
    {
        Color color = blackOverlayImage.color;
        color.a = alpha;
        blackOverlayImage.color = color;
    }
}
