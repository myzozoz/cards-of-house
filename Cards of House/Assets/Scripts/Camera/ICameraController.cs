using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraController
{
    public void AddShot(string targetName, Vector3 targetPosition, Vector3 cameraRotation, Vector3 cameraOffset);
    public bool HasShot(string shotName);
    public void TransitionTo(string shot);
}
