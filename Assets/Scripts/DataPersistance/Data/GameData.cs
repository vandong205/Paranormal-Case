using UnityEngine;

public class GameData
{
    public string currentSceneName;
    public SerializableVector3 playerPosition;
    // Saved area reference used by LightManager/TeleportGateManager
    public int currentAreaRef = -1;
    public bool isNewGame = true;
    public GameData()
    {
    }
    
}
