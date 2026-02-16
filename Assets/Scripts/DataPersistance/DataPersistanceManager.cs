using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Unity.Burst.Intrinsics;

public class DataPersistanceManager : MonoBehaviour
{
    [SerializeField] private string fileName = "gameData.json"; 
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
    
    public void SaveGame()
    {
        Debug.Log("Save Game");
        // capture current active scene name into save data
        if (gameData == null) gameData = new GameData();
        Debug.Log("Data objects count: " + dataPersistanceObjects.Count);
        gameData.currentSceneName = SceneManager.GetActiveScene().name;
        dataPersistanceObjects = FindAllDataPersistanceObjects();
        foreach (IDataPersistance dataPersistanceObj in dataPersistanceObjects)
        {
            dataPersistanceObj.SaveData(ref gameData);
        }
        dataHandler.Save(gameData);
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
    private List<IDataPersistance> FindAllDataPersistanceObjects()
    {
        IEnumerable<IDataPersistance> dataPersistanceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistance>();
        return new List<IDataPersistance>(dataPersistanceObjects);
    }
}
