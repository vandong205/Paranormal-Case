using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MainCanvas : MonoBehaviour
{
    [Header("Global")]
    [SerializeField] private Volume GlobalVolume;
    [SerializeField] private StaticUIController staticUIController;
    [SerializeField] private ScreenTransitonController screenTransitionController;

    [SerializeField] private MessageBox messageBox;
    [SerializeField] private Notification notification;
    [SerializeField] private PanelUI panelUI;

    public PanelUI PanelUI=>panelUI;
    public Notification Notification=> notification;

    public MessageBox MessageBox=>messageBox;
    public ScreenTransitonController ScreenTransitionController => screenTransitionController;
    public StaticUIController StaticUIController => staticUIController;
    private ColorAdjustments  colorAdjustments;
    private MainCanvas() { }
    private static MainCanvas instance;
    public static MainCanvas Instance
    {
        get
        {
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        if(staticUIController == null)
            staticUIController = FindFirstObjectByType<StaticUIController>();   
        if (GlobalVolume != null)
        {
            if (GlobalVolume.profile.TryGet(out ColorAdjustments ca))
            {
                colorAdjustments = ca;
            }
            else
            {
                Debug.LogWarning("MainCanvas: No ColorAdjustments found in GlobalVolume profile.");
            }
        }
        else
        {
            GlobalVolume = FindFirstObjectByType<Volume>();
        }
    }
    public void SetGlobalSaturation(float saturation)
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value = saturation;
        }
    }
    public void RegisterMessageBox(MessageBox mb)
    {
        this.messageBox = mb;
    }
}
