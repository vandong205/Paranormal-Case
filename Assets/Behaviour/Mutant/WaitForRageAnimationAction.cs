using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitForRageAnimation", story: "Wait until rage animation in [Monster] done", category: "Action", id: "ca70eed4233443bf7ad94195e1c76637")]
public partial class WaitForRageAnimationAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Monster;
    private Animator m_Animator;
    private bool m_RageStarted;

    protected override Status OnStart()
    {
        m_RageStarted = false;
        m_Animator = null;

        if (Monster == null || Monster.Value == null)
        {
            LogFailure("No Monster assigned.");
            return Status.Failure;
        }

        return Status.Running;
    }


    protected override Status OnUpdate()
    {
        if (m_Animator == null)
        {
            m_Animator = Monster.Value.GetComponent<Animator>();
            if (m_Animator == null)
            {
                LogFailure("No Animator found on Monster.");
                return Status.Failure;
            }
        }

        AnimatorStateInfo stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);

    // Rage bắt đầu
        if (stateInfo.IsName("rage"))
        {
            m_RageStarted = true;

            // Rage CHƯA xong
            if (stateInfo.normalizedTime < 1f)
                return Status.Running;
        }

        // Chỉ Success khi rage đã từng chạy và đã kết thúc
        if (m_RageStarted)
            return Status.Success;

        return Status.Running;
    }


    protected override void OnEnd()
    {
    }
}

