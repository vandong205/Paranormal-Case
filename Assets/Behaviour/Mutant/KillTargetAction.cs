using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Kill Target", story: "Kill [Target]", category: "Action", id: "68b4588ce6c3b97907c0d22800b90306")]
public partial class KillTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart()
    {
        if (Target == null || Target.Value == null)
        {
            LogFailure("No Target assigned.");
            return Status.Failure;
        }
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        ILivingEntity livingEntity = Target.Value.GetComponent<ILivingEntity>();
        if (livingEntity == null)
        {
            LogFailure("Target does not implement ILivingEntity.");
            return Status.Failure;
        }
        livingEntity.Die();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

