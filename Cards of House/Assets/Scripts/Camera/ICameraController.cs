using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraController
{
    public void AddShot(string targetName, Transform targetTransform, Vector3 cameraRotation, Vector3 cameraOffset);
    public void AddShot(string targetName, Transform targetTransform, string baseShot);
    public bool HasShot(string shotName);
    public void TransitionTo(string shot);
    public void TransitionToFollow(Transform transform, string shot);
}
