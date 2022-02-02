using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRim : MonoBehaviour, IClickable
{
    [SerializeField]
    private Color readyColor;
    [SerializeField]
    private Color hoverColor;
    [SerializeField]
    private Color selectedColor;

    private MeshRenderer mr;
    private Material mat;

    public enum State
    {
        Offline,
        Selectable,
        Inactive,
        Locked
    };
    private State state = State.Offline;
    private IBoard board;

    // Start is called before the first frame update
    void Start()
    {
        board = GameData.Instance.BoardObject.GetComponent<IBoard>();

        mr = gameObject.GetComponent<MeshRenderer>();
        mat = mr.material;
        mr.enabled = false;
        UpdateColor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
    }

    private void UpdateColor()
    {
        if (!mr.enabled)
            return;

        switch (state)
        {
            case State.Selectable:
                mat.SetColor("_Color", readyColor);
                break;
            case State.Inactive:
                mat.SetColor("_Color", readyColor);
                break;
            case State.Locked:
                mat.SetColor("_Color", selectedColor);
                break;
        }
    }

    public void HandleClick()
    {
        if (state == State.Selectable)
        {
            //Debug.Log($"Clicked spawn tile at position {transform.position}");
            board.TrySelectSpawnTile(this);
            UpdateColor();
        }
    }

    void OnMouseOver()
    {
        //Debug.Log("Mouse is hovering over me...");
        //Debug.Log($"{mat.GetColor("_Color")} should be {hoverColor} but my state is ${state.ToString()}");
        if (state == State.Selectable)
        {
            mat.SetColor("_Color", hoverColor);
        }
    }

    void OnMouseExit()
    {
        UpdateColor();
    }

    public Vector3 Position
    {
        get { return this.transform.position; }
    }

    public SpawnRim.State SpawnState
    {
        get { return state; }
        set
        {
            switch (value)
            {
                case State.Offline:
                    mr.enabled = false;
                    break;
                case State.Selectable:
                    if (!mr.enabled)
                        mr.enabled = true;
                    break;
                case State.Locked:
                    if (!mr.enabled)
                        mr.enabled = true;
                    break;
            }
            state = value;
            UpdateColor();
        }
    }

    public SpawnRim Ref
    {
        get { return this; }
    }
}
