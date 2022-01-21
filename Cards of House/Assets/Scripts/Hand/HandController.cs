using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour, IHand
{
    private bool ready;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Ready()
    {
        return ready;
    }

    public void Initialize()
    {
        ready = false;
    }

    public void End()
    {
        //clean up
    }
}
