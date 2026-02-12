using System;
using Unity.Properties;
using UnityEngine;
namespace Unity.Behavior{
    [Serializable, GeneratePropertyBag]
    [Condition(
        name: "Check Horizontal Distance",
        category: "Conditions",
        story: "horizontal distance between [Transform] and [Target] [Operator] [Threshold]",
        id: "d4c1a3e7b8f042b49a6f9c2e1b0a9f77"
    )]
    internal partial class CheckHorizontalDistanceCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<Transform> Transform;
        [SerializeReference] public BlackboardVariable<Transform> Target;

        // 🔑 PHẢI dùng cái này để có dropdown > < >= <= ==
        [Comparison(comparisonType: ComparisonType.All)]
        [SerializeReference] public BlackboardVariable<ConditionOperator> Operator;

        [SerializeReference] public BlackboardVariable<float> Threshold;

        public override bool IsTrue()
        {
            if (Transform.Value == null || Target.Value == null)
            {
                return false;
            }

            // Get collider bounds from Transform
            Collider collider = Transform.Value.GetComponent<Collider>();
            if (collider == null)
            {
                return false;
            }

           

            // Check horizontal distance (XZ plane)
            Vector3 a = Transform.Value.position;
            Vector3 b = Target.Value.position;

            Vector2 aXZ = new Vector2(a.x, a.z);
            Vector2 bXZ = new Vector2(b.x, b.z);

            float horizontalDistance = Vector2.Distance(aXZ, bXZ);

            return ConditionUtils.Evaluate(horizontalDistance, Operator, Threshold.Value);
        }
    }
    
}
