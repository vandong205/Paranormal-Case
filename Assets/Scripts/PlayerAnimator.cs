using System;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator; 
    public Action OnDeathAnimationComplete; 
    public void OnDeathAnimaionDone()
    {
        MainCanvas.Instance.SetGlobalSaturation(-100f);
        GameManager.Instance.PlayerCamera.ZoomIn(() =>
        {
            MainCanvas.Instance.ScreenTransitionController.PlayTransition(TransitionType.VerticalShutter, TransitionDirection.In,OnDeathAnimationComplete);
        });
    }
    public void DoDie()
    {
        animator.SetTrigger("Die");
    }
    public void DoRevive()
    {
        animator.SetTrigger("Revive");
    }
}
