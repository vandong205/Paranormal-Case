using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using DG.Tweening;
using System;

[RequireComponent(typeof(CinemachineCamera))]
public class CameraController : MonoBehaviour, IOnSceneReady
{
    [Header("Zoom Setting")]
    [SerializeField] private float zoomInSize = 4f;
    [SerializeField] private float zoomDuration = 0.5f;
    [SerializeField] private Ease zoomEase = Ease.OutQuad;

    private CinemachineCamera cinemachineCamera;

    private float defaultOrthographicSize;
    private Tween zoomTween;

    private void Awake()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();

        // lưu giá trị ban đầu
        defaultOrthographicSize = cinemachineCamera.Lens.OrthographicSize;
    }
    void Start()
    {
        GameManager.Instance.RegisterPlayerCamera(this);
    }
    public IEnumerator OnSceneReady()
    {
        yield return new WaitForEndOfFrame();
    }

    public void ZoomIn(Action onComplete = null)
    {
        zoomTween?.Kill();

        zoomTween = DOTween.To(
            () => cinemachineCamera.Lens.OrthographicSize,
            x => cinemachineCamera.Lens.OrthographicSize = x,
            zoomInSize,
            zoomDuration
        ).SetEase(zoomEase)
        .OnComplete(() =>
        {
            DOVirtual.DelayedCall(1f, () => onComplete?.Invoke());
        });
    }

    public void ZoomOut()
    {
        zoomTween?.Kill();

        zoomTween = DOTween.To(
            () => cinemachineCamera.Lens.OrthographicSize,
            x => cinemachineCamera.Lens.OrthographicSize = x,
            defaultOrthographicSize,
            zoomDuration
        ).SetEase(zoomEase);
    }
}