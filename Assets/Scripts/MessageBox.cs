using UnityEngine;
using TMPro;

public class MessageBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private GameObject uiContainer;

    private void Awake()
    {
        if (uiContainer != null)
        {
            uiContainer.SetActive(false);
        }
    }
    void Start()
    {
        MainCanvas.Instance.RegisterMessageBox(this);
    }
    public static void Show(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        MainCanvas canvas = MainCanvas.Instance;
        if (canvas == null || canvas.MessageBox == null)
        {
            Debug.LogWarning(text);
            return;
        }

        canvas.MessageBox.ShowInternal(text);
    }

    private void ShowInternal(string text)
    {
        if (messageText != null)
        {
            messageText.text = text;
        }

        if (uiContainer != null)
        {
            uiContainer.SetActive(true);
        }
    }

    public void Hide()
    {
        if (uiContainer != null)
        {
            uiContainer.SetActive(false);
        }
    }
}
