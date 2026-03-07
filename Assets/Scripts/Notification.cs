using UnityEngine;
using TMPro;
using System.Collections;

public class Notification : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] textPool = new TextMeshProUGUI[3];

    [SerializeField] private float fadeInTime = 0.25f;
    [SerializeField] private float showTime = 2f;
    [SerializeField] private float fadeOutTime = 0.4f;

    private Coroutine[] runningCoroutines;
    private int nextIndex = 0;

    private void Awake()
    {
        runningCoroutines = new Coroutine[textPool.Length];

        for (int i = 0; i < textPool.Length; i++)
        {
            SetAlpha(textPool[i], 0);
            textPool[i].gameObject.SetActive(false);
        }
    }

    public void Show(string message)
    {
        int index = nextIndex;
        nextIndex = (nextIndex + 1) % textPool.Length;

        TextMeshProUGUI text = textPool[index];

        if (runningCoroutines[index] != null)
        {
            StopCoroutine(runningCoroutines[index]);
        }

        text.text = message;
        text.gameObject.SetActive(true);

        SetAlpha(text, 0);

        text.transform.SetAsLastSibling();

        runningCoroutines[index] = StartCoroutine(ShowRoutine(text, index));
    }

    private IEnumerator ShowRoutine(TextMeshProUGUI text, int index)
    {
        float t = 0;

        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            SetAlpha(text, t / fadeInTime);
            yield return null;
        }

        SetAlpha(text, 1);

        yield return new WaitForSeconds(showTime);

        t = 0;

        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            SetAlpha(text, 1 - (t / fadeOutTime));
            yield return null;
        }

        SetAlpha(text, 0);
        text.gameObject.SetActive(false);

        runningCoroutines[index] = null;
    }

    private void SetAlpha(TextMeshProUGUI text, float a)
    {
        Color c = text.color;
        c.a = a;
        text.color = c;
    }
}