using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RandomTeleport", story: "Random teleport [Self] between [SpawnPoints] , set [PatrolPoints]", category: "Action", id: "303d641a8cbd7c5d8aaf8f0c45f3f7db")]
public partial class RandomTeleportAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<List<GameObject>> SpawnPoints;
    [SerializeReference] public BlackboardVariable<List<GameObject>> PatrolPoints;
    protected override Status OnStart()
    {
        int randomIndex = UnityEngine.Random.Range(0, SpawnPoints.Value.Count);
        GameObject selectedSpawnPoint = SpawnPoints.Value[randomIndex];
        Self.Value.transform.position = selectedSpawnPoint.transform.position;
       if(selectedSpawnPoint.TryGetComponent<NPCSpawnPoint>(out var patrolComponent))
        {
            PatrolPoints.Value.Clear();
            foreach (var point in patrolComponent.patrolPoints)
            {
                PatrolPoints.Value.Add(point);
            }
        }
        return Status.Success;
        
    }
}

