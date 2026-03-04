using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;
using System;
using System.Collections;
[RequireComponent(typeof(CinemachineConfiner3D))]
[Serializable]
public class CameraConfinerData
{
    public int areaID;
    public BoxCollider confinerCollider;
}

[RequireComponent(typeof(CinemachineConfiner3D))]
[RequireComponent(typeof(CinemachineCamera))]
public class CameraController : MonoBehaviour, IOnSceneReady
{
    public IEnumerator OnSceneReady()
    {
        yield return new WaitForEndOfFrame(); 
    }
}
