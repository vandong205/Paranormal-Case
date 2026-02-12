using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckSameFloorWithTarget", story: "[Target] is on same floor with [Monster]", category: "Conditions", id: "3a6960812552b482a68e9360c30aa96b")]
public partial class CheckSameDiffWithTargetCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<GameObject> Monster;

    public override bool IsTrue()
    {
        if (!Monster.Value.TryGetComponent<Collider>(out var collider))
        {
            return false;
        }
        Bounds bounds = collider.bounds;
        Vector3 targetPos = Target.Value.transform.position;
        if (targetPos.y < bounds.min.y || targetPos.y > bounds.max.y)
        {
                return false; 
        }
        return true;
    }

}
