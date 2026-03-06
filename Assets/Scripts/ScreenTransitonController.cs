using System.Collections;
using UnityEngine;

public enum TransitionType
{
    Iriss,
    VerticalShutter
}

public enum TransitionDirection
{
    In,
    Out
}

[RequireComponent(typeof(Animator))]
public class ScreenTransitonController : MonoBehaviour
{
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private float waitForStateTimeout = 2f;

    void Awake()
    {
        if (transitionAnimator == null)
        {
            transitionAnimator = GetComponent<Animator>();
            if (transitionAnimator == null)
            {
                Debug.LogError("ScreenTransitionController: No Animator found on the GameObject.");
            }
        }
    }

    public void PlayTransition(TransitionType type, TransitionDirection direction, System.Action onComplete = null)
    {
        if (transitionAnimator == null)
        {
            Debug.LogError("ScreenTransitionController: transitionAnimator is null.");
            return;
        }

        string triggerName = GetTriggerName(type, direction);
        string stateName = GetAnimationStateName(type, direction);

        transitionAnimator.SetTrigger(triggerName);
        StartCoroutine(WaitAnimationFinish(stateName, onComplete));
    }

    private IEnumerator WaitAnimationFinish(string stateName, System.Action onComplete)
    {
        float elapsed = 0f;
        while (!transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            elapsed += Time.deltaTime;
            if (elapsed >= waitForStateTimeout)
            {
                Debug.LogWarning($"ScreenTransitionController: Timeout waiting for state '{stateName}'.");
                yield break;
            }

            yield return null;
        }

        while (transitionAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        Debug.Log("Chay su kien sau khi chay animation screen");
        onComplete?.Invoke();
    }

    private string GetTriggerName(TransitionType type, TransitionDirection direction)
    {
        return $"{type}_{direction}";
    }

    private string GetAnimationStateName(TransitionType type, TransitionDirection direction)
    {
        if (type == TransitionType.VerticalShutter)
            return direction == TransitionDirection.In ? "verticalshutterin" : "verticalshutterout";

        return $"{type}{direction}".ToLowerInvariant();
    }
}
