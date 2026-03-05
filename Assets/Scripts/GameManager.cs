using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static event Action<float> OnLoadingProgressChanged;
    private TransitionLayerController currentTransition;
    [SerializeField] private string NewGameScene = "Chapter2";
    [SerializeField] private CameraController playerCamera;


    public CameraController PlayerCamera => playerCamera;
    private string GamePlayScene;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        QualitySettings.vSyncCount = 1;   
        Application.targetFrameRate = -1;
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        LoadGameData();
    }
    public void RegisterPlayerCamera(CameraController cam)
    {
        playerCamera = cam;
    }   
    public void LoadGameData()
    {
        StartCoroutine(LoadRoutine());
    }

    private IEnumerator LoadRoutine()
{
    DataPersistanceManager.Instance.LoadGameData();

    GamePlayScene =
        !string.IsNullOrEmpty(DataPersistanceManager.Instance.gameData?.currentSceneName)
        ? DataPersistanceManager.Instance.gameData.currentSceneName
        : NewGameScene;

    yield return StartCoroutine(SmoothProgress(0f, 0.1f, 0.25f));

    // ===== Gameplay Scene =====
    AsyncOperation gameplayLoad =
        SceneManager.LoadSceneAsync(GamePlayScene, LoadSceneMode.Single);

    gameplayLoad.allowSceneActivation = false;

    while (gameplayLoad.progress < 0.9f)
    {
        float mapped = Mathf.Lerp(0.1f, 0.6f, gameplayLoad.progress / 0.9f);
        OnLoadingProgressChanged?.Invoke(mapped);
        yield return null;
    }

    gameplayLoad.allowSceneActivation = true;

    while (!gameplayLoad.isDone)
        yield return null;

    // ===== UI Scene =====
    AsyncOperation uiLoad =
        SceneManager.LoadSceneAsync("MainUI", LoadSceneMode.Additive);

    while (!uiLoad.isDone)
        yield return null;

    OnLoadingProgressChanged?.Invoke(0.8f);
}

    private IEnumerator SmoothProgress(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Lerp(from, to, Mathf.Clamp01(t / duration));
            OnLoadingProgressChanged?.Invoke(p);
            yield return null;
        }
        OnLoadingProgressChanged?.Invoke(to);
    }
    public void ReportProgress(float value)
    {
        OnLoadingProgressChanged?.Invoke(value);
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != GamePlayScene) return;

        StartCoroutine(InitSceneRoutine());
    }

    private IEnumerator InitSceneRoutine()
    {
        yield return null; // đảm bảo Start() đã chạy

        DataPersistanceManager.Instance.InitGameData();
        yield  return SceneInitializer.InitializeScene();
        Debug.Log("Scene Initialized Safely");
        if (currentTransition != null)
        {
            Debug.Log("Closing transition layer");
            currentTransition.CloseCurtain();
        }else{
            Debug.LogWarning("No transition layer registered, skipping curtain close.");
            }
    }
 public void RegisterTransition(TransitionLayerController controller)
{
    currentTransition = controller;
}

}
