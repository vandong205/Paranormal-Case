using UnityEngine;
using System.Collections;

using System.Linq;

public static class SceneInitializer
{
    public static IEnumerator InitializeScene()
    {
        var initializables =
            Object.FindObjectsByType<MonoBehaviour>(
                FindObjectsSortMode.None)
            .OfType<IOnSceneReady>();

        foreach (var obj in initializables)
        {
            yield return obj.OnSceneReady();
        }

        // đảm bảo physics + camera ổn định
        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame();
    }
}