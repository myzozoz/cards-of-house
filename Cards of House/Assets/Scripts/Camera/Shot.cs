using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Shot
{
    public string Name { get; set; }
    public Vector3 TargetPosition { get; set; }
    public Vector3 CameraRotation { get; set; }
    public Vector3 CameraOffset { get; set; }
}
