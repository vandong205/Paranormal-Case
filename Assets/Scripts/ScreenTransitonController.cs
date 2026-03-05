
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
        string state = GetStateName(type, direction);
        transitionAnimator.SetTrigger(state);
        StartCoroutine(WaitAnimationFinish(state, onComplete));
    }
    private IEnumerator WaitAnimationFinish(string stateName, System.Action onComplete)
    {
        // đợi animator thực sự vào state
        while (!transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            yield return null;

        // đợi animation chạy xong
        while (transitionAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;

        onComplete?.Invoke();
    }
    private string GetStateName(TransitionType type, TransitionDirection direction)
    {
        return $"{type}_{direction}";
    }
}

