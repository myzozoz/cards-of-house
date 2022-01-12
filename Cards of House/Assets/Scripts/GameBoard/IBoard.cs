using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IBoard
{
    public void Step();
    public List<IUnit> GetUnitsOnBoard();
    public List<ITarget> GetTargetsOnBoard();
    public Vector3Int GetUnitLocation(System.Guid unitId);
    public Grid GetGrid();
    public Tilemap GetTilemap();
    public void ResetPathTiles();
    public void Deregister(System.Guid unitId);
}
