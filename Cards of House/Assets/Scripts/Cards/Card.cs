using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour, ICard, IClickable
{
    [SerializeField]
    private Color unselectedColor;
    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Color readyToSubmitColor;

    private Material outlineMat;

    private bool selected;
    private System.Guid id;
    private ITable table;

    void Awake()
    {
        id = System.Guid.NewGuid();
    }

    // Start is called before the first frame update
    void Start()
    {
        outlineMat = gameObject.GetComponent<MeshRenderer>().material;
        table = transform.parent.GetComponent<ITable>();
        if (table == null)
        {
            Debug.Log("Could not find ITable component");
        }
        SetSelected(false);
        outlineMat.SetFloat("_CustomAlpha", .6f);
        UpdateColor();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateColor();
    }

    public void SetSelected(bool sel)
    {
        selected = sel;
    }

    public bool IsSelected()
    {
        return selected;
    }

    public void HandleClick()
    {
        if (table.TryToggleSelect(id))
        {
            SetSelected(!selected);
        }
    }

    public System.Guid GetId()
    {
        return id;
    }

    public void OnMouseEnter()
    {
        outlineMat.SetFloat("_CustomAlpha", 1f);
    }

    public void OnMouseExit()
    {
        outlineMat.SetFloat("_CustomAlpha", .6f);
    }

    private void UpdateColor()
    {
        if (selected)
        {
            outlineMat.SetColor("_Color", table.ReadyToSubmit() ? readyToSubmitColor : selectedColor);
        }
        else
        {
            outlineMat.SetColor("_Color", unselectedColor);
        }
    }
}
