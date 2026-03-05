using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;

public class DataPersistanceManager : MonoBehaviour
{
    [SerializeField] private string fileName = "gameData.json"; 
    [SerializeField] private float autoSaveInterval = 600f;
    private DataPersistanceManager() { }
    public GameData gameData;
    private FileDataHandler dataHandler;
    List<IDataPersistance> dataPersistanceObjects = new();
    private static DataPersistanceManager instance;
    public static DataPersistanceManager Instance
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
            Debug.LogWarning("Multiple instances of DataPersistanceManager found! Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);

    }
    void Start()
    {
        StartCoroutine(AutoSaveLoop()); 
    }
    private System.Collections.IEnumerator AutoSaveLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveInterval);
            RuntimeSaveGame();
        }
    }
    public void NewGame()
    {
        Debug.Log("New Game");
        gameData = new GameData
        {
            currentSceneName = "Chapter2"
        };
    }   
    public void LoadGameData()
    {
        Debug.Log("Load Game Data");    
        gameData = dataHandler.Load();
         if (gameData == null)
        {
            Debug.LogWarning("No game data found. Starting new game.");
            NewGame();
        }
    }
    public void InitGameData()
    {
        dataPersistanceObjects = FindAllDataPersistanceObjects();
        if(gameData.isNewGame)
        {
            gameData.isNewGame = false;
            return; // leave all objects at default data in scene for new game, only load saved data for existing game
           
        }
        
        
        Debug.Log("Init Game");
        int totalObjects = dataPersistanceObjects != null ? dataPersistanceObjects.Count : 0;
        if (totalObjects == 0)
        {
            GameManager.Instance.ReportProgress(1f);
            return;
        }

        int loaded = 0;
        foreach (IDataPersistance dataPersistanceObj in dataPersistanceObjects)
        {
            dataPersistanceObj.LoadData(gameData);
            Debug.Log("Loaded data for: " + dataPersistanceObj.GetType().Name);
            loaded++;
            float localNorm = (float)loaded / totalObjects; // 0..1
            float mapped = Mathf.Lerp(0.8f, 1f, localNorm);
            GameManager.Instance.ReportProgress(mapped);
        }
    }
    public void RuntimeSaveGame()
    {
        StartCoroutine(RuntimeSaveRoutine());
    }

    private System.Collections.IEnumerator RuntimeSaveRoutine()
{
    float minDisplayTime = 1.5f;
    float startTime = Time.time;

    MainCanvas.Instance.StaticUIController.AutoSavingIcon.Appear();

    yield return StartCoroutine(SaveGameAsync());

    float elapsed = Time.time - startTime;

    if (elapsed < minDisplayTime)
        yield return new WaitForSeconds(minDisplayTime - elapsed);

    MainCanvas.Instance.StaticUIController.AutoSavingIcon.Disappear();
}
    private System.Collections.IEnumerator SaveGameAsync()
{
    gameData ??= new GameData();

    gameData.currentSceneName = SceneManager.GetActiveScene().name;

    dataPersistanceObjects = FindAllDataPersistanceObjects();

    foreach (IDataPersistance dataPersistanceObj in dataPersistanceObjects)
    {
        dataPersistanceObj.SaveData(ref gameData);
    }

    // cho Unity 1 frame để không block
    yield return null;

    dataHandler.Save(gameData);
}
    private void OnApplicationQuit()
    {
        StartCoroutine(SaveGameAsync());
    }
    private List<IDataPersistance> FindAllDataPersistanceObjects()
    {
        IEnumerable<IDataPersistance> dataPersistanceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistance>();
        return new List<IDataPersistance>(dataPersistanceObjects);
    }
}
