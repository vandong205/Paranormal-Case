using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Check Target Death", story: "[Target] is not death", category: "Conditions", id: "75ee3596c9d7d209f30f82cbac662c9d")]
public partial class CheckTargetDeathCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    public override bool IsTrue()
    {
        if (!Target.Value.TryGetComponent<ILivingEntity>(out var LivingEntityTarget))
        {
            MessageBox.Show("Target khong hop le de kiem tra trang thai song.");
            return false;
        }
        return LivingEntityTarget.IsAlive;
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
