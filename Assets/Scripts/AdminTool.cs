
using TMPro;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Profiling;

public class AdminTool : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI FPS;
    [SerializeField] private TextMeshProUGUI MemoryUsage;
    [SerializeField] private float notificationFadeDuration = 2f;
    private Coroutine notificationCoroutine;
    private SerializableVector3 lastPlayerPosition;
    private int frameCount = 0;
    private float elapsedTime = 0f;
    void Update()
    {
        frameCount++;
        elapsedTime += Time.unscaledDeltaTime;
        if (elapsedTime >= 1f)
        {
            float fps = frameCount / elapsedTime;
            if (FPS != null)
                FPS.text = $"FPS: {fps:F1}";
            if (MemoryUsage != null)
            {
                long memory = Profiler.GetTotalAllocatedMemoryLong();
                MemoryUsage.text = $"Memory: {memory / (1024f * 1024f):F1} MB";
            }
            frameCount = 0;
            elapsedTime = 0f;
        }

    }
    void Awake()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
            if (player == null)
            {
                MessageBox.Show("Khong tim thay Player trong scene.");
            }else{
                speedText.text = player.MoveSpeed.ToString("F1");
            }
        }
        if(DataPersistanceManager.Instance != null)
        {
            lastPlayerPosition = DataPersistanceManager.Instance.gameData.playerPosition ;
        }
        else
        {
            SetNotification("Warning: No GameData found, player position will not be saved.");
        }

        // Load saved AdminTool settings (if any)
        LoadAdminToolData();
    }
    void Start()
    {
        if (notificationText != null)
        {
            var c = notificationText.color;
            c.a = 0f;
            notificationText.color = c;
            notificationText.text = string.Empty;
        }
    }
    /// <summary>
    /// Call this to show a notification: it will appear after ~2s, then fade out and clear.
    /// </summary>
    public void SetNotification(string message)
    {
        if (notificationText == null) return;
        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
            notificationCoroutine = null;
        }
        notificationCoroutine = StartCoroutine(NotificationRoutine(message));
    }

    private System.Collections.IEnumerator NotificationRoutine(string message)
    {

        notificationText.text = message;
        Color start = notificationText.color;
        start.a = 1f;
        notificationText.color = start;

        float t = 0f;
        while (t < notificationFadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t / notificationFadeDuration);
            Color c = notificationText.color;
            c.a = a;
            notificationText.color = c;
            yield return null;
        }

        var endColor = notificationText.color;
        endColor.a = 0f;
        notificationText.color = endColor;
        notificationText.text = string.Empty;
        notificationCoroutine = null;
    }
    public void IncreasePlayerSpeed()
    {
        if (player != null)
        {
            player.MoveSpeed += 1f;
            SetNotification("Player speed increased to: " + player.MoveSpeed);
            speedText.text = player.MoveSpeed.ToString("F1");
        }
    }
    public void DecreasePlayerSpeed()
    {
        if (player != null)
        {
            player.MoveSpeed = Mathf.Max(0f, player.MoveSpeed - 1f);
            SetNotification("Player speed decreased to: " + player.MoveSpeed);
            speedText.text = player.MoveSpeed.ToString("F1");
        }
    }
    public void TeleportPlayerToLastPosition()
    {
        if (player != null)
        {
            player.transform.position = lastPlayerPosition.ToVector3();
            SetNotification("Player teleported to last saved position.");
        }
    }

    private void OnApplicationQuit()
    {
        SaveAdminToolData();
    }
    public void KillPlayer()
    {
        if (player != null)
        {
            player.Die();
            SetNotification("Player killed.");
        }
    }
    [Serializable]
    private class AdminToolData
    {
        public float playerMoveSpeed;
        public SerializableVector3 lastPlayerPosition;
    }

    private void SaveAdminToolData()
    {
        try
        {
            var data = new AdminToolData
            {
                playerMoveSpeed = player != null ? player.MoveSpeed : 0f,
                lastPlayerPosition = this.lastPlayerPosition
            };

            string json = JsonUtility.ToJson(data, true);
            string path = Path.Combine(Application.persistentDataPath, "AdminToolData.json");
            File.WriteAllText(path, json);
            Debug.Log($"AdminTool: Saved data to {path}");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Luu du lieu AdminTool that bai: " + ex.Message);
        }
    }
    public void SaveGame()
    {
        if (DataPersistanceManager.Instance != null)
        {
            DataPersistanceManager.Instance.RuntimeSaveGame();
        }
    }
    private void LoadAdminToolData()
    {
        try
        {
            string path = Path.Combine(Application.persistentDataPath, "AdminToolData.json");
            if (!File.Exists(path)) return;
            string json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<AdminToolData>(json);
            if (data == null) return;

            if (player != null)
            {
                player.MoveSpeed = data.playerMoveSpeed;
                if (speedText != null)
                    speedText.text = player.MoveSpeed.ToString("F1");
            }
            Debug.Log($"AdminTool: Loaded data from {path}");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Tai du lieu AdminTool that bai: " + ex.Message);
        }
    }
}
