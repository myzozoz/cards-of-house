using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit: ITarget
{
    public void Execute(Command command);
    public TargetType TargetMode { get; set; }
    public bool CanAct();
    public Vector3 GetPosition();
    public Transform GetTransform();
}
