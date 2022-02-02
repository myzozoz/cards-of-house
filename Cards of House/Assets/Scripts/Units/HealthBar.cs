using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Color color;
    [SerializeField]
    private Color backgroundColor;
    [SerializeField]
    private float fill = 1f;
    [SerializeField]
    private GameObject foreground;
    [SerializeField]
    private GameObject background;

    private Material fgMat;
    private Material bgMat;

    // Start is called before the first frame update
    void Start()
    {
        fgMat = foreground.GetComponent<MeshRenderer>().material;
        bgMat = background.GetComponent<MeshRenderer>().material;
        bgMat.SetFloat("_Fill", 1f);
        bgMat.SetColor("_Color", backgroundColor);
        fgMat.SetColor("_Color", color);
    }

    // Update is called once per frame
    void Update()
    {
        fgMat.SetFloat("_Fill", fill);
    }

    public float Fill
    {
        get { return fill; }
        set { fill = Mathf.Clamp(value, 0f,1f); }
    }

    public void SetVisible(bool val)
    {
        foreach (Transform childT in transform)
        {
            transform.gameObject.SetActive(val);
        }
    }

    public bool IsVisible()
    {
        return gameObject.activeInHierarchy;
    }
}
