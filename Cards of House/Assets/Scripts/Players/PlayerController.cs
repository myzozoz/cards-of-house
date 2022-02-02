using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayer
{
    public string playerName;
    public int team;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string Name
    {
        get { return playerName; }
    }

    public int Team
    {
        get { return team; }
    }

    public Vector3Int GetLocation()
    {
        return new Vector3Int(0, 0, 0);
    }

    public void TakeDamage(float val)
    {
        Debug.Log($"{transform.name} taking {val} damage");
    }

}
