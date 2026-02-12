using UnityEngine;

public class LightManager : MonoBehaviour
{
    public LightController[] lightControllers;
    public int defaultAreaID = -1;

    void Start()
    {
        lightControllers = GetComponentsInChildren<LightController>(true);
        if (lightControllers == null || lightControllers.Length == 0)
        {
            Debug.LogWarning("No LightController references assigned in LightManager.");
        }
        if(defaultAreaID != -1)
        {
            SetAreaLightsActive(defaultAreaID, true);
        }
    }

    public void SetAreaLightsActive(int areaID, bool enabled)
    {
        if (lightControllers == null) return;
        bool foundAny = false;
        foreach (var controller in lightControllers)
        {
            if (controller != null && controller.GetAreaID() == areaID)
            {
                controller.SetAllLightsActive(enabled);
                foundAny = true;
            }
        }
        if (!foundAny)
        {
            Debug.LogWarning($"No LightController found for area {areaID}.");
        }
    }
}
