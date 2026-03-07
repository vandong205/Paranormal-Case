using UnityEngine;

public class StaticUIController : MonoBehaviour
{
    [SerializeField] private AutoSavingIcon autoSavingIcon;
    public AutoSavingIcon AutoSavingIcon => autoSavingIcon;
    void Start()
    {
        if (autoSavingIcon == null)
            autoSavingIcon = GetComponentInChildren<AutoSavingIcon>();
    }
}
