using UnityEngine;

[System.Serializable]
public struct Shot
{
    public string Name;
    public Transform TargetTransform;
    public Vector3 CameraRotation;
    public Vector3 CameraOffset;
    
    public Shot(string name, Transform targetTransform, Vector3 cameraRotation, Vector3 cameraOffset)
    {
        Name = name;
        TargetTransform = targetTransform;
        CameraRotation = cameraRotation;
        CameraOffset = cameraOffset;
    }
}
