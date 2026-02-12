
using UnityEngine;

public class LightController : MonoBehaviour
{
    private Light[] lights;
    [SerializeField] int AreaID = -1;
    void Start()
    {
        lights = GetComponentsInChildren<Light>(true);
        if(lights == null || lights.Length == 0)
        {
            Debug.LogWarning("No Light components found in children.");
            return;
        }
    }
    public void SetAllLightsActive(bool enabled)
    {
        if (lights == null) return;

        foreach (var light in lights)
        {
            Debug.Log($"Setting light {light.name} enabled={enabled}");
            light.gameObject.SetActive(true);
            light.enabled = enabled;
        }
    }
    public int GetAreaID()
    {
        return AreaID;
    }
}
