using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour, ICard, IClickable
{
    [SerializeField]
    private Color hoverColor;
    [SerializeField]
    private Color selectedColor;

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
        Debug.Log($"Card parent: {transform.parent}");
        table = transform.parent.GetComponent<ITable>();
        if (table == null)
        {
            Debug.Log("Could not find ITable component");
        }
        SetSelected(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetSelected(bool sel)
    {
        selected = sel;

        outlineMat.SetFloat("_CustomAlpha", selected ? .8f : 0f);
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

    public void OnMouseOver()
    {
        Debug.Log($"Mouse is over {this}");
        if (selected)
        {
            outlineMat.SetColor("_Color", selectedColor);
            outlineMat.SetFloat("_CustomAlpha", 1f);
        }
        else
        {
            outlineMat.SetColor("_Color", hoverColor);
            outlineMat.SetFloat("_CustomAlpha", 0.8f);
        }
    }

    public void OnMouseExit()
    {
        SetSelected(selected);
    }
}
