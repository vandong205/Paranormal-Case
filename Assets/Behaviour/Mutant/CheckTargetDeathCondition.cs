using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Check Target Death", story: "[Target] is death", category: "Conditions", id: "75ee3596c9d7d209f30f82cbac662c9d")]
public partial class CheckTargetDeathCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    public override bool IsTrue()
    {
        ILivingEntity LivingEntityTarget = Target.Value.GetComponent<ILivingEntity>();
        if (LivingEntityTarget == null){
            Debug.LogError("LivingEntity Target is null in CheckTargetDeathCondition");
            return false;
        }
        return !LivingEntityTarget.IsAlive;
    }

    public override void OnStart()
    {
        if (Target == null || Target.Value == null)
            return;
        

    }

    public override void OnEnd()
    {
    }
}
