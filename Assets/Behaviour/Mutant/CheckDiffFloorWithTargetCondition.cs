using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckDiffFloorWithTarget", story: "[Target] is on difference floor with [Monster]", category: "Conditions", id: "557c19046c7fe0ce5240991e950f4f37")]
public partial class CheckDiffFloorWithTargetCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<GameObject> Monster;

    public override bool IsTrue()
    {
        Collider collider = Monster.Value.GetComponent<Collider>();
         Bounds bounds = collider.bounds;
        Vector3 targetPos = Target.Value.transform.position;
        if (targetPos.y < bounds.min.y || targetPos.y > bounds.max.y)
        {
                return true; 
        }
        return false;
    }
}
