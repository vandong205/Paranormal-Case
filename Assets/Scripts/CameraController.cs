using UnityEngine;
using Unity.Cinemachine;
public class CameraController : MonoBehaviour
{
    [SerializeField] CinemachineConfiner3D cinemachineConfiner;
    void Awake()
    {
        if (cinemachineConfiner == null)
        {
            cinemachineConfiner = GetComponent<CinemachineConfiner3D>();
        }
    }
    public void SetConfiner(Collider confiner)
    {
        cinemachineConfiner.BoundingVolume = confiner;
    }   
}
