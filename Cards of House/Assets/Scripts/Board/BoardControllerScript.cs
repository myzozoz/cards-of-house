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
    private HashSet<System.Guid> deregisterBuffer = new HashSet<System.Guid>();
    private HashSet<SpawnRim> spawnTiles = new HashSet<SpawnRim>();
    private List<Vector3Int> enemySpawns = new List<Vector3Int>();
    private Dictionary<ICard, SpawnRim> selectedSpawns = new Dictionary<ICard, SpawnRim>();
    private HashSet<Vector3Int> reservedTiles = new HashSet<Vector3Int>();

    private ICameraController cam;
    private IHand hand;
    [SerializeField]
    private short roundsPerStage = 12;
    [SerializeField]
    private short spawnEnemiesAfterRounds = 5;
    [SerializeField]
    private short enemySpawnCount = 3;
    private short roundsSimulated;
    private bool ready;
    [SerializeField]
    private Transform spawnTransform;
    [SerializeField]
    private GameObject enemyUnit;

    private HashSet<AvatarUnit> playerAvatars = new HashSet<AvatarUnit>();
    private HashSet<AvatarUnit> enemyAvatars = new HashSet<AvatarUnit>();
    private AIController ai;


    Dictionary<(int,int),TileBase> initialBoardConfig = new Dictionary<(int, int), TileBase>();

    // Start is called before the first frame update
    void Start()
    {
        grid = transform.GetComponent<Grid>();
        tilemap = transform.GetComponentInChildren<Tilemap>();
        hand = GameData.Instance.HandObject.GetComponent<IHand>();
        ai = GameData.Instance.AI;

        _InitAvatars();
        _InitEnemySpawns();

        if (tilemap == null)
        {
            Debug.Log("No Tilemap found, please check that one such exists");
        }
        UpdateUnits();
        UpdateTargets();
        ready = false;
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
        StopAllCoroutines();
    }

    public int Round
    {
        get { return roundsSimulated+1; }
    }

    public void Simulate(Command command)
    {
        if (GameData.Instance.CurrentStage != Stage.Simulate)
        {
            Debug.Log("Not simulation time yet");
            return;
        }

        SubmitCommand(0, command);
        if (roundsSimulated % spawnEnemiesAfterRounds == 0)
        {
            SpawnEnemies();
        }
        SubmitTarget(1, ai.GetTarget());
        SubmitCommand(1, ai.GetCommand());
        roundsSimulated++;
        CheckWinLose();
        if (roundsSimulated >= roundsPerStage)
        {
            ready = true;
        }
    }

    public void SubmitCommand(int team, Command command)
    {
        // if we are currently in a state where we can do this action
        if (GameData.Instance.CurrentStage != Stage.Simulate)
        {
            return;
        }

        foreach (IUnit u in GetUnitsOnBoard(team))
        {
            u.Execute(command);
        }
    }

    public void SubmitTarget(int team, TargetType targetType)
    {
        foreach (IUnit u in GetUnitsOnBoard(team))
        {
            u.TargetMode = targetType;
        }
    }

    private void CheckWinLose()
    {
        if (playerAvatars.Count <= 0)
        {
            GameData.Instance.WState = WinState.Lost;
        }
        else if (enemyAvatars.Count <= 0)
        {
            GameData.Instance.WState = WinState.Won;
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

    public List<IUnit> GetUnitsOnBoard(int team)
    {
        List<IUnit> uList = new List<IUnit>(units.Values);
        List<IUnit> teamList = new List<IUnit>();
        foreach (IUnit u in uList)
        {
            if (u.Team == team)
            {
                teamList.Add(u);
            }
        }
        return teamList;
    }

    public List<ITarget> GetTargetsOnBoard()
    {
        return new List<ITarget>(targets.Values);
    }

    public List<ITarget> GetTargetsOnBoard(int team)
    {
        List<ITarget> tList = new List<ITarget>(units.Values);
        List<ITarget> teamList = new List<ITarget>();
        foreach (ITarget t in tList)
        {
            if (t.Team == team)
            {
                teamList.Add(t);
            }
        }
        return teamList;
    }

    public Vector3Int GetUnitLocation(System.Guid unitId)
    {
        return units[unitId].GetLocation();
    }

    public Grid GetGrid()
    {
        return GetComponent<Grid>();
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

        if (reservedTiles.Contains(loc))
        {
            return false;
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
        spawnTiles.Clear();
        for (int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
        {
            for (int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
            {
                Vector3Int temp = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(temp) && tilemap.GetTile(temp).name == "SpawnTileAsset")
                {
                    SpawnRim sr = tilemap.GetInstantiatedObject(temp).GetComponentInChildren<SpawnRim>();
                    spawnTiles.Add(sr);
                }
            }
        }
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

    private void _InitAvatars()
    {
        playerAvatars.Clear();
        enemyAvatars.Clear();

        List<GameObject> pas = new List<GameObject>(GameObject.FindGameObjectsWithTag("PlayerAvatar"));
        List<GameObject> eas = new List<GameObject>(GameObject.FindGameObjectsWithTag("EnemyAvatar"));

        foreach (GameObject go in pas)
        {
            playerAvatars.Add(go.GetComponent<AvatarUnit>());
        }

        foreach (GameObject go in eas)
        {
            enemyAvatars.Add(go.GetComponent<AvatarUnit>());
        }
    }

    public void RegisterAvatarDeath(AvatarUnit au)
    {
        if (playerAvatars.Contains(au))
        {
            playerAvatars.Remove(au);
        } else if (enemyAvatars.Contains(au))
        {
            enemyAvatars.Remove(au);
        }
        CheckWinLose();
    }

    private void _InitEnemySpawns()
    {
        enemySpawns.Clear();
        for (int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
        {
            for (int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
            {
                Vector3Int temp = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(temp) && tilemap.GetTile(temp).name == "SpawnEnemy")
                {
                    enemySpawns.Add(temp);
                }
            }
        }
    }

    private void SpawnEnemies()
    {
        //Randomize spawn locations
        //Spawn enemies
        List<Vector3Int> availableSpawns = new List<Vector3Int>();

        foreach (Vector3Int v in enemySpawns)
        {
            if (IsFree(v))
            {
                availableSpawns.Add(v);
            }
        }

        //Debug.Log($"Available spawn tiles: {availableSpawns.Count}/{enemySpawns.Count}");

        HashSet<int> indexes = new HashSet<int>();
        while (indexes.Count < enemySpawnCount && indexes.Count < availableSpawns.Count)
        {
            indexes.Add(Random.Range(0, enemySpawns.Count));
        }

        foreach (int i in indexes)
        {
            SpawnUnit(enemyUnit, enemySpawns[i]);
        }
        UpdateUnits();
        UpdateTargets();
    }

    public bool ReserveTile(Vector3Int t)
    {
        if (!reservedTiles.Contains(t))
        {
            reservedTiles.Add(t);
            return true;
        }

        return false;
    }

    public bool ReleaseTile(Vector3Int t)
    {
        if (reservedTiles.Contains(t))
        {
            reservedTiles.Remove(t);
            return true;
        }
        return false;
    }
}
