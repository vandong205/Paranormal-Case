using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UpdateLostTime", story: "Update [LostTime] if NOT [CanSeeTarget]", category: "Action", id: "c0f4b9f3e8fe0d13b808f8d7d3bce143")]
public partial class UpdateLostTimeAction : Action
{
    [SerializeReference] public BlackboardVariable<float> LostTime;
    [SerializeReference] public BlackboardVariable<bool> CanSeeTarget;

    protected override Status OnUpdate()
    {
        if (!CanSeeTarget.Value)
        {
            LostTime.Value += Time.deltaTime;
        }
        else
        {
            LostTime.Value = 0f;
        }
        return Status.Running;
    }
}

