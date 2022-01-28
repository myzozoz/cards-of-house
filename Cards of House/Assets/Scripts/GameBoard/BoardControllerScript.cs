using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class BoardControllerScript : MonoBehaviour, IBoard
{
    public int width;
    public int depth;
    public int forbiddenTiles;
    public TileBase defaultTile;
    public TileBase forbiddenTile;

    private Grid grid;
    private Tilemap tilemap;
    private Dictionary<System.Guid, IUnit> units = new Dictionary<System.Guid, IUnit>();
    private Dictionary<System.Guid, ITarget> targets = new Dictionary<System.Guid, ITarget>();
    private Queue<System.Guid> actionQueue = new Queue<System.Guid>();
    private HashSet<System.Guid> deregisterBuffer = new HashSet<System.Guid>();
    private HashSet<SpawnRim> spawnTiles = new HashSet<SpawnRim>();
    private Dictionary<ICard, SpawnRim> selectedSpawns = new Dictionary<ICard, SpawnRim>();


    private ICameraController cam;
    private IHand hand;
    private Vector3 unitCamRotation = new Vector3(50f, 0f, 0f);
    private Vector3 unitCamOffset = new Vector3(0f, 8.5f, -6f);
    [SerializeField]
    private short roundsPerStage = 12;
    private short roundsSimulated;
    private bool ready;
    [SerializeField]
    private Transform spawnTransform;

    Dictionary<(int,int),TileBase> initialBoardConfig = new Dictionary<(int, int), TileBase>();
    // Start is called before the first frame update
    void Start()
    {
        grid = transform.GetComponent<Grid>();
        tilemap = transform.GetComponentInChildren<Tilemap>();
        hand = GameData.Instance.HandObject.GetComponent<IHand>();
        if (tilemap == null)
        {
            Debug.Log("No Tilemap found, please check that one such exists");
        }
        UpdateUnits();
        UpdateTargets();
        //UpdateSpawns();
        foreach (IUnit unit in units.Values)
        {
            actionQueue.Enqueue(unit.GetId());
        }
        ready = true;
        roundsSimulated = 0;

        initialBoardConfig.Clear();
        for (int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
        {
            for (int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
            {
                Vector3Int temp = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(temp))
                {
                    initialBoardConfig.Add((x, y), tilemap.GetTile(temp));
                }
            }
        }
    }

    public bool Ready()
    {
        return ready;
    }

    public void Initialize()
    {
        ready = false;
        roundsSimulated = 0;

        foreach (ICard c in selectedSpawns.Keys)
        {
            SpawnUnit(c.GetSpawnableUnit(), grid.WorldToCell(selectedSpawns[c].Position));
            c.Delete();
        }
        selectedSpawns.Clear();

        foreach (SpawnRim sr in spawnTiles)
        {
            sr.SpawnState = SpawnRim.State.Offline;
        }

        UpdateUnits();
        UpdateTargets();
    }

    public void End()
    {
        // clean up
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //AddRandomForbiddenTiles(forbiddenTiles);
            Debug.Log($"Units in queue {actionQueue.Count}");
            Debug.Log($"Ids in deregister buffer {deregisterBuffer.Count}");
        }

    }

    public void Step()
    {
        System.Guid currentId = actionQueue.Dequeue();
        if (!deregisterBuffer.Contains(currentId)) {
            IUnit current = units[currentId];
            if (!cam.HasShot(currentId.ToString()))
            {
                cam.AddShot(currentId.ToString(), current.GetTransform(), $"Unit_{current.GetTeam()}");
            }
            cam.TransitionTo(currentId.ToString());
            //Debug.Log("Now in turn: " + current);
            //Do actions
            current.Execute();
            //Back to the queue
            actionQueue.Enqueue(currentId);
            roundsSimulated++;
        } else {
            UpdateTargets();
            UpdateUnits();
            deregisterBuffer.Remove(currentId);
        }

        Debug.Log($"Rounds simulated: {roundsSimulated}/{roundsPerStage} (ready:{ready})");
        if (roundsSimulated >= roundsPerStage)
        {
            ready = true;
        }
    }

    private void OverrideBoxFill2D(Vector3Int start, TileBase tile, int startX, int startY, int endX, int endY)
    {
        int dirX = startX < endX ? 1 : -1;
        int dirY = startY < endY ? 1 : -1;
        int cols = 1 + Mathf.Abs(endX - startX);
        int rows = 1 + Mathf.Abs(endY - startY);

        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3Int pos = start + new Vector3Int(startX, startY, 0) + new Vector3Int(dirX * x, dirY * y, 0);
                tilemap.SetTile(pos, tile);
            }
        }
    }

    private void AddRandomForbiddenTiles(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3Int pos = new Vector3Int(Random.Range(0, width), Random.Range(1, depth - 1), 0);
            while (tilemap.GetTile(pos).name == forbiddenTile.name)
            {
                Debug.Log("Tile already at " + pos);
                pos = new Vector3Int(Random.Range(0, width), Random.Range(1, depth - 1), 0);
                Debug.Log("New pos: " + pos);
            }

            Debug.Log("Setting tile at " + pos);
            tilemap.SetTile(pos, forbiddenTile);
        }
    }

    private void InitializeMap()
    {
        OverrideBoxFill2D(new Vector3Int(0, 0, 0), defaultTile, 0, 0, width - 1, depth - 1);
        AddRandomForbiddenTiles(forbiddenTiles);
    }

    private void UpdateUnits()
    {
        units.Clear();
        List<IUnit> unitList = new List<IUnit>(transform.GetComponentsInChildren<IUnit>());
        foreach (IUnit unit in unitList)
        {
            if (unit != null && unit.IsAlive() && unit.CanAct())
            {
                units.Add(unit.GetId(), unit);
            }
        }
    }

    private void UpdateTargets()
    {
        targets.Clear();
        List<ITarget> targetList = new List<ITarget>(transform.GetComponentsInChildren<ITarget>());
        foreach (ITarget target in targetList)
        {
            if (target != null && target.IsAlive())
            {
                targets.Add(target.GetId(), target);
            }
        }
        //Debug.Log($"A total of {targetList.Count} targets found");
    }

    public List<IUnit> GetUnitsOnBoard()
    {
        return new List<IUnit>(units.Values);
    }

    public List<ITarget> GetTargetsOnBoard()
    {
        return new List<ITarget>(targets.Values);
    }

    public Vector3Int GetUnitLocation(System.Guid unitId)
    {
        return units[unitId].GetLocation();
    }

    public Grid GetGrid()
    {
        return grid;
    }

    public Tilemap GetTilemap()
    {
        return tilemap;
    }

    public bool IsFree(Vector3Int loc)
    {
        if (!tilemap.HasTile(loc))
        {
            return false;
        }

        if (tilemap.GetTile(loc).name == "Forbidden")
        {
            return false;
        }

        foreach (ITarget t in targets.Values)
        {
            if (t.GetLocation() == loc)
            {
                return false;
            }
        }

        return true;
    }

    public void ResetPathTiles()
    {
        for (int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
        {
            for (int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
            {
                Vector3Int temp = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(temp) && initialBoardConfig.ContainsKey((x, y)))
                {
                    tilemap.SetTile(temp, initialBoardConfig[(x,y)]);
                }
            }
        }
    }

    public void Deregister(System.Guid unitId)
    {
        deregisterBuffer.Add(unitId);
        UpdateUnits();
        UpdateTargets();
    }

    public void SpawnUnit(GameObject unit, Vector3Int spawnLocation)
    {
        GameObject go = Instantiate(unit, tilemap.GetCellCenterWorld(spawnLocation), Quaternion.Euler(0,0,0), this.transform);
        IUnit unitController = go.GetComponent<IUnit>();
        if (unitController != null)
        {
            actionQueue.Enqueue(unitController.GetId());
        }
        UpdateUnits();
        UpdateTargets();
    }

    public void SetCam(ICameraController camController)
    {
        cam = camController;
    }

    public Transform GetSpawnCenterTransform()
    {
        return spawnTransform;
    }

    public void EnableSpawnSelection()
    {
        Debug.Log($"Spawn selection enabled, spawn tiles held: {spawnTiles.Count}");
        ICard c = hand.GetSelectedCard();
        if (selectedSpawns.ContainsKey(c))
        {
            //unselect spawn
            selectedSpawns[c].SpawnState = SpawnRim.State.Selectable;
            //remove spawn from selected spawns
            selectedSpawns.Remove(c);
        }

        foreach (SpawnRim sr in spawnTiles)
        {
            if (sr.SpawnState == SpawnRim.State.Offline || sr.SpawnState == SpawnRim.State.Inactive)
                sr.SpawnState = SpawnRim.State.Selectable;
        }
    }

    public void DisableSpawnSelection()
    {
        foreach (SpawnRim sr in spawnTiles)
        {
            if (sr.SpawnState == SpawnRim.State.Selectable)
                sr.SpawnState = SpawnRim.State.Inactive;
        }
    }

    public void UpdateSpawns()
    {
        Debug.Log($"Spawn tiles before update: {spawnTiles.Count}");
        spawnTiles.Clear();
        for (int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
        {
            for (int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
            {
                Vector3Int temp = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(temp) && tilemap.GetTile(temp).name == "SpawnTileAsset")
                {
                    Debug.Log($"Getting tile at {temp}");
                    SpawnRim sr = tilemap.GetInstantiatedObject(temp).GetComponentInChildren<SpawnRim>();
                    spawnTiles.Add(sr);
                }
            }
        }
        Debug.Log($"Spawn tiles after update: {spawnTiles.Count}");
    }

    public void TrySelectSpawnTile(SpawnRim tile)
    {
        // We can select tile if tile is unselected
        if (!selectedSpawns.ContainsValue(tile))
        {
            selectedSpawns.Add(hand.GetSelectedCard(), tile);
            hand.LockSelectedCard();
            tile.SpawnState = SpawnRim.State.Locked;
        }
    }
}
