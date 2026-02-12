using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TrackTarget", story: "[Self] tracking [Target] in [range] , set [CanSeeTarget]", category: "Action", id: "b7731b672911aa3932a1656e428b59d3")]
public partial class TrackTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Range;
    [SerializeReference] public BlackboardVariable<bool> CanSeeTarget;
    private bool _hasSeenTarget = false;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var selfGO = Self?.Value;
        var targetGO = Target?.Value;

        if (selfGO == null || targetGO == null)
            return Status.Running;

        var range = Range != null ? Range.Value : 0f;

        // Determine collider bounds (support 3D and 2D colliders)
        Bounds bounds = new(selfGO.transform.position, Vector3.zero);
        if (selfGO.TryGetComponent<Collider>(out var col3))
        {
            bounds = col3.bounds;
        }
        else
        {
            if (selfGO.TryGetComponent<Collider2D>(out var col2))
                bounds = col2.bounds;
        }

        var minY = bounds.min.y;
        var maxY = bounds.max.y;

        var selfPos = selfGO.transform.position;
        var targetPos = targetGO.transform.position;

        // Horizontal check: X-axis distance within range
        var horizDist = Mathf.Abs(targetPos.x - selfPos.x);
        var inHorizontal = horizDist <= range;

        // Vertical check: target Y between collider bottom and top
        var inVertical = targetPos.y >= minY && targetPos.y <= maxY;

        var canSee = inHorizontal && inVertical;
        if (CanSeeTarget != null)
        {
            CanSeeTarget.Value = canSee;
        }


        // If target is seen for the first time, set range to 100
        if (!_hasSeenTarget && canSee)
        {
            _hasSeenTarget = true;
            if (Range != null)
                Range.Value = 100f;
        }

        // Nếu player thoát khỏi tầng (không còn nhìn thấy target), reset range về 5 và _hasSeenTarget về false
        if (_hasSeenTarget && !canSee)
        {
            _hasSeenTarget = false;
            if (Range != null)
                Range.Value = 20f;
        }

        return Status.Running;
    }
}

