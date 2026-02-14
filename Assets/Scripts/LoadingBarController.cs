using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBarController : MonoBehaviour
{
    [SerializeField] private Slider loadingBarFill;
    [SerializeField] private TextMeshProUGUI loadingTextPercent;
    [SerializeField] private float smoothSpeed = 3f;

    private float targetValue = 0f;

    private void OnEnable()
    {
        GameManager.OnLoadingProgressChanged += SetProgress;
    }

    private void OnDisable()
    {
        GameManager.OnLoadingProgressChanged -= SetProgress;
    }

    private void Update()
    {
        loadingBarFill.value = Mathf.MoveTowards(
            loadingBarFill.value,
            targetValue,
            smoothSpeed * Time.deltaTime
        );

        loadingTextPercent.text =
            Mathf.RoundToInt(loadingBarFill.value * 100f) + "%";
    }

    private void SetProgress(float progress)
    {
        targetValue = progress;
    }
}
