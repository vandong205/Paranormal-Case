using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogBox : MonoBehaviour
{
    public static DialogBox Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private Image portraitImage;
    [SerializeField] private GameObject layout;
    [SerializeField] Button skipButton;
    [SerializeField] float skipButtonDelay = 3f;
    [SerializeField] private TextMeshProUGUI skipButtonLabel;

    private Coroutine typingCoroutine;
    private Coroutine skipEnableCoroutine;
    private TextItem currentItem;
    private bool hasCurrentItem = false;
    private bool canSkip = false;
    private bool isTextFullyShown = false;

    private struct TextItem
    {
        public string Text;
        public Sprite Portrait;
        public float TypingSpeed;
    }

    private readonly System.Collections.Generic.Queue<TextItem> textQueue = new System.Collections.Generic.Queue<TextItem>();
    private bool isRunningSequence = false;

    // Called once when the sequence of texts starts
    public System.Action OnSequenceStarted;
    // Called once when the sequence of texts ends (queue empty)
    public System.Action OnSequenceEnded;
    // Called before each text is shown (passes the text)
    public System.Action<string> OnBeforeShow;
    // Called after each text is shown (passes the text)
    public System.Action<string> OnAfterShow;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (skipButton != null)
            skipButton.interactable = false;
        if (skipButtonLabel != null)
            skipButtonLabel.text = string.Empty;
    }

    // Push a text item into the queue. If not currently running, starts the sequence.
    public void PushText(string text, Sprite portrait = null, float typingSpeed = 30f)
    {
        var item = new TextItem
        {
            Text = text ?? string.Empty,
            Portrait = portrait,
            TypingSpeed = typingSpeed
        };

        textQueue.Enqueue(item);

        // If not already running a sequence, start it
        if (!isRunningSequence)
        {
            isRunningSequence = true;
            OnSequenceStarted?.Invoke();

            if (layout != null)
                layout.SetActive(true);

            ShowTextNext();
        }
    }

    // Backwards-compatible alias for older code that called ShowText.
    // This will push a single text and start the sequence if needed.
    public void ShowText(string text, Sprite portrait = null, float typingSpeed = 50f)
    {
        PushText(text, portrait, typingSpeed);
    }

    // Show the next text from the queue. If queue is empty will end the sequence and hide the layout.
    public void ShowTextNext()
    {
        if (textQueue.Count == 0)
        {
            // sequence finished
            isRunningSequence = false;
            OnSequenceEnded?.Invoke();
            Hide();
            return;
        }

        var item = textQueue.Dequeue();
        currentItem = item;
        hasCurrentItem = true;
        isTextFullyShown = false;
        canSkip = false;
        if (skipEnableCoroutine != null)
            StopCoroutine(skipEnableCoroutine);
        // initialize label to starting countdown value
        if (skipButtonLabel != null)
            skipButtonLabel.text = skipButtonDelay > 0f ? skipButtonDelay.ToString("F1") : "Skip ";
        skipEnableCoroutine = StartCoroutine(EnableSkipAfterDelay());

        if (portraitImage != null && item.Portrait != null)
            portraitImage.sprite = item.Portrait;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        // Notify before showing this text
        OnBeforeShow?.Invoke(item.Text);

        if (item.TypingSpeed <= 0f)
        {
            if (textMesh != null)
                textMesh.text = item.Text;

            // After showing instantly
            OnAfterShow?.Invoke(item.Text);
            isTextFullyShown = true;

            // Do not auto-advance - wait for user to call ShowTextNext() or Skip()
            return;
        }

        typingCoroutine = StartCoroutine(TypeTextCoroutine(item.Text, item.TypingSpeed));
    }

    public void Hide()
    {
        if (layout != null)
            layout.SetActive(false);

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        if (skipEnableCoroutine != null)
        {
            StopCoroutine(skipEnableCoroutine);
            skipEnableCoroutine = null;
        }
        canSkip = false;
        if (skipButton != null)
            skipButton.interactable = false;
        if (skipButtonLabel != null)
            skipButtonLabel.text = string.Empty;
    }

    private IEnumerator TypeTextCoroutine(string text, float charsPerSecond)
    {
        if (textMesh == null)
            yield break;

        textMesh.text = string.Empty;
        float delay = 1f / Mathf.Max(1f, charsPerSecond);

        for (int i = 0; i < text.Length; i++)
        {
            textMesh.text += text[i];
            yield return new WaitForSeconds(delay);
        }

        typingCoroutine = null;

        // Notify after showing this text
        OnAfterShow?.Invoke(text);
        isTextFullyShown = true;

        // Do not auto-advance - wait for user to call ShowTextNext() or Skip()
    }

    private IEnumerator EnableSkipAfterDelay()
    {
        if (skipButtonDelay <= 0f)
        {
            canSkip = true;
            if (skipButton != null)
                skipButton.interactable = true;
            if (skipButtonLabel != null)
                skipButtonLabel.text = "Skip";
            skipEnableCoroutine = null;
            yield break;
        }

        float remaining = skipButtonDelay;
        while (remaining > 0f)
        {
            if (skipButtonLabel != null)
                skipButtonLabel.text = remaining.ToString("F1");
            yield return null;
            remaining -= Time.deltaTime;
            if (!isRunningSequence)
            {
                // aborted
                yield break;
            }
        }

        canSkip = true;
        if (skipButton != null)
            skipButton.interactable = true;
        if (skipButtonLabel != null)
            skipButtonLabel.text = "Skip";
        skipEnableCoroutine = null;
    }

    // Skip current typing or advance to next item.
    // If currently typing, completes the current text immediately and advances to the next.
    // If not typing (text fully shown), simply advances to the next item.
    public void Skip()
    {
        // If nothing is active and no queued items, nothing to do
        if (!isRunningSequence)
            return;

        // If skipping is not yet allowed, ignore
        if (!canSkip)
            return;

        if (typingCoroutine != null)
        {
            // finish current immediately (first skip press)
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;

            if (hasCurrentItem)
            {
                if (textMesh != null)
                    textMesh.text = currentItem.Text;

                OnAfterShow?.Invoke(currentItem.Text);
                isTextFullyShown = true;
                // keep hasCurrentItem true until player advances
            }

            // Do not auto-advance here; the next skip will advance.
            return;
        }

        // Not typing right now.
        // If the current text is fully shown, advance to the next on this press.
        if (isTextFullyShown)
        {
            isTextFullyShown = false;
            hasCurrentItem = false;
            ShowTextNext();
            return;
        }

        // Otherwise nothing to do.
    }
}
