using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IBoard: IStage
{
    public void Simulate();
    public List<IUnit> GetUnitsOnBoard();
    public List<ITarget> GetTargetsOnBoard();
    public Vector3Int GetUnitLocation(System.Guid unitId);
    public Grid GetGrid();
    public Tilemap GetTilemap();
    public bool IsFree(Vector3Int loc);
    public void ResetPathTiles();
    public void Deregister(System.Guid unitId);
    public void SpawnUnit(GameObject unit, Vector3Int spawnLocation);
    public void SetCam(ICameraController camController);
    public Transform GetSpawnCenterTransform();
    public void EnableSpawnSelection();
    public void DisableSpawnSelection();
    public void TrySelectSpawnTile(SpawnRim tile);
    public void UpdateSpawns();
    public int Round { get; }
    public void RegisterAvatarDeath(AvatarUnit au);
}
