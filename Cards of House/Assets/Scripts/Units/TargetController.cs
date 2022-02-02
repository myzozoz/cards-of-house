using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour, ITarget
{
    public TargetType targetType;
    public int team;
    public float maxHealth = 100f;
    
    
    protected float health;
    protected HealthBar hb;
    protected IBoard board;
    protected System.Guid id;
    protected Grid grid;
    protected bool alive = true;

    void Awake()
    {
        id = System.Guid.NewGuid();
    }

    // Start is called before the first frame update
    protected void Start()
    {
        health = maxHealth;
        board = GameData.Instance.BoardObject.GetComponent<IBoard>();
        grid = board.GetGrid();

        hb = GetComponentInChildren<HealthBar>();
        hb.SetVisible(false);
    }


    void FixedUpdate()
    {
        hb.Fill = health / maxHealth;
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
        if (board == null)
        {
            board = GameData.Instance.BoardObject.GetComponent<IBoard>();
        }

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
        if (!hb.IsVisible())
            hb.SetVisible(true);
        
        
    }

    public void Die()
    {
        //Death animations and such
        board.RegisterAvatarDeath(this as AvatarUnit);
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
