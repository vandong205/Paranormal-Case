using UnityEngine;
using System.Collections;

public class AutoSavingIcon : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
    }

    public void Appear()
    {
        StartFade(1f);
    }

    public void Disappear()
    {
        StartFade(0f);
    }

    private void StartFade(float target)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(Fade(target));
    }

    private IEnumerator Fade(float target)
    {
        float start = canvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = target;
    }
}