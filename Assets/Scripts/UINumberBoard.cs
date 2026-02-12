using UnityEngine;

public class UINumberBoard : MonoBehaviour
{
    [SerializeField] GameObject layout;
    public static UINumberBoard Instance { get; private set; }

    private ITextInputReceiver currentReceiver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void Show()
    {
        layout.SetActive(true);
    }
    public void Hide()
    {
        layout.SetActive(false);
    }
    public void Mount(ITextInputReceiver receiver)
    {
        currentReceiver = receiver;
    }

    public void Unmount()
    {
        currentReceiver = null;
    }

    // Button called by UI
    public void PressKey(string key)
    {
        if (currentReceiver == null) return;

        if (key == "BS")
            currentReceiver.OnBackspace();
        else if (key == "OK")
            currentReceiver.OnSubmit();
        else if (!string.IsNullOrEmpty(key))
        {
            currentReceiver.OnCharInput(key[0]);
        }
           
    }
    public void Test()
    {
    }
}
