using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour, ITarget
{
    public TargetType targetType;
    public int team;
    public float health = 100f;

    protected IBoard board;
    protected System.Guid id;
    protected Grid grid;
    protected bool alive = true;

    void Awake()
    {
        id = System.Guid.NewGuid();
    }

    // Start is called before the first frame update
    void Start()
    {
        board = transform.parent.GetComponent<IBoard>();
        grid = board.GetGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (health <= 0f)
        {
            Die();
        }
    }

    public TargetType GetTargetType()
    {
        return targetType;
    }

    public int GetTeam()
    {
        return team;
    }

    public Vector3Int GetLocation()
    {
        if (grid == null)
        {
            grid = board.GetGrid();
        }

        return grid.WorldToCell(transform.position);
    }

    public System.Guid GetId()
    {
        return id;
    }

    public void TakeDamage(float val)
    {
        //Debug.Log($"{transform.name} taking {val} damage");
        health -= val;
        health = health > 0f ? health : 0f;
    }

    public void Die()
    {
        //Death animations and such
        Debug.Log("Dying...");
        alive = false;
        board.Deregister(id);
        Destroy(gameObject);
    }

    public bool IsAlive()
    {
        return alive;
    }
}
