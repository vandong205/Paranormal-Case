

public static class GameDatabase
{
    public static GameDB Instance { get; private set; } 
    public static void Initialize(GameDB gameDB)
    {
        Instance = gameDB;
    }
}
