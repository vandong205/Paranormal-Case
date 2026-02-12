using TMPro;
using UnityEngine;

public class DoorSign : MonoBehaviour, ITextInputReceiver
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject layout;
    [SerializeField] TextMeshProUGUI number;
    public bool editable = false;
    private System.Text.StringBuilder inputBuilder = new System.Text.StringBuilder(2);
    private ImmobileDoor ownerDoor;
    // Deactivate the layout GameObject
    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (number == null)
            number = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void Show()
    {
        if (layout != null)
        {
            layout.SetActive(true);
            animator.Play("show");
            if (editable)
            {
                UINumberBoard.Instance.Unmount();
                // Mount to UINumberBoard so keys will be routed here
                UINumberBoard.Instance.Mount(this);
                UINumberBoard.Instance.Show();
                // reset input and display placeholder
                inputBuilder.Clear();
                RefreshDisplay();
            }
        }
            
    }
    public void Hide()
    {
        if (animator != null)
            animator.SetTrigger("Hide");

        UINumberBoard.Instance.Unmount();
        UINumberBoard.Instance.Hide();
    }

    public void HideLayout()
    {
        if (layout != null)
            layout.SetActive(false);
        if (editable)
            UINumberBoard.Instance.Unmount();
    }

    private void RefreshDisplay()
    {
        if (number == null) return;

        if (inputBuilder.Length == 0)
        {
            number.text = "_ _";
            return;
        }

        if (inputBuilder.Length == 1)
        {
            number.text = inputBuilder[0] + " _";
            return;
        }

        // length >= 2
        number.text = string.Concat(inputBuilder[0], inputBuilder[1]);
    }

    // ITextInputReceiver implementation
    public void OnCharInput(char character)
    {
        if (!editable) return;
        if (inputBuilder.Length >= 2) return;
        Debug.Log(gameObject.name + " receive text input: " + character);
        inputBuilder.Append(character);
        RefreshDisplay();
    }

    public void OnBackspace()
    {
        if (!editable) return;
        if (inputBuilder.Length == 0) return;
        inputBuilder.Length = inputBuilder.Length - 1;
        RefreshDisplay();
    }

    public void OnSubmit()
    {
        if (!editable) return;
        if (ownerDoor.CheckSignNumber(GetCurrentNumber()))
        {
            Hide();
        }
    }
    public void BindOwner(ImmobileDoor door)
    {
        ownerDoor = door;
    }

    public int GetCurrentNumber()
    {
        // Prefer the live input buffer if any
        string s = inputBuilder.Length > 0 ? inputBuilder.ToString() : (number != null ? number.text : string.Empty);

        if (string.IsNullOrWhiteSpace(s))
            return -1;

        // Clean common placeholders/spacing
        s = s.Replace("_", string.Empty).Replace(" ", string.Empty);

        if (int.TryParse(s, out int value))
            return value;

        return -1;
    }
    public void ClearData()
    {
           inputBuilder.Clear();
        RefreshDisplay();

    }
    public void SetNumber(int number)
    {
        if (this.number != null)
        {
            this.number.text = number.ToString();
        }
    }

}
