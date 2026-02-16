using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private GameDB gameDB;
    void Awake()
    {
        GameDatabase.Initialize(gameDB);
    }
}
