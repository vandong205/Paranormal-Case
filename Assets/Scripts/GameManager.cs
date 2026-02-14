using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static event Action<float> OnLoadingProgressChanged;

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
    void Start()
    {
        LoadGameData();
    }
    public void LoadGameData()
    {
        StartCoroutine(LoadRoutine());
    }

    private System.Collections.IEnumerator LoadRoutine()
    {
        int totalSteps = 5;

        for (int i = 1; i <= totalSteps; i++)
        {
            yield return new WaitForSeconds(0.5f);

            float progress = (float)i / totalSteps;
            OnLoadingProgressChanged?.Invoke(progress);
        }
        OnLoadingDataComplete();
    }
    private void OnLoadingDataComplete()
    {
        SceneManager.LoadScene("Chapter2");
    }
}
