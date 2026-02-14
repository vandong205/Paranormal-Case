using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static event Action<float> OnLoadingProgressChanged;
    private TransitionLayerController currentTransition;
    [SerializeField] private string sceneName = "Chapter2";
    private string targetSceneName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
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

    public void LoadGameData()
    {
        StartCoroutine(LoadRoutine());
    }

    private IEnumerator LoadRoutine()
    {
        // Phase 1: File load (0% -> 10%)
        DataPersistanceManager.Instance.LoadGameData();
        // decide which scene to load: prefer saved scene name
        targetSceneName = !string.IsNullOrEmpty(DataPersistanceManager.Instance.gameData?.currentSceneName)
            ? DataPersistanceManager.Instance.gameData.currentSceneName
            : sceneName;
        yield return StartCoroutine(SmoothProgress(0f, 0.1f, 0.25f));

        // Phase 2: Scene load (10% -> 80%)
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            float normalized = asyncLoad.progress / 0.9f; // 0..1
            float mapped = Mathf.Lerp(0.1f, 0.8f, normalized);
            OnLoadingProgressChanged?.Invoke(mapped);
            yield return null;
        }

        // ensure UI reaches 80%
        OnLoadingProgressChanged?.Invoke(0.8f);
        asyncLoad.allowSceneActivation = true;

        // wait until scene activated
        while (!asyncLoad.isDone)
            yield return null;

        // Phase 3 (injection) will run in InitSceneRoutine and DataPersistanceManager will report 80->100%
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
        if (scene.name != targetSceneName) return;

        StartCoroutine(InitSceneRoutine());
    }

    private IEnumerator InitSceneRoutine()
    {
        yield return null; // đảm bảo Start() đã chạy

        DataPersistanceManager.Instance.InitGameData();

        Debug.Log("Scene Initialized Safely");
        if (currentTransition != null)
        {
            currentTransition.CloseCurtain();
        }
    }
 public void RegisterTransition(TransitionLayerController controller)
{
    currentTransition = controller;
}

}
