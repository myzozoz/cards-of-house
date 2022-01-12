using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITarget : ITeam
{
    public TargetType GetTargetType();
    public Vector3Int GetLocation();
    public void TakeDamage(float val);
    public System.Guid GetId();
    public bool IsAlive();
}
