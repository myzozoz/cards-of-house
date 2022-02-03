using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour, ICard
{
    [SerializeField]
    private Color offlineColor;
    [SerializeField]
    private Color unselectedColor;
    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Color readyToSubmitColor;
    [SerializeField]
    private AnimationCurve transitionCurve;
    [SerializeField]
    private float transitionTime = 1.0f;
    [SerializeField]
    private GameObject SpawnedUnit;

    private Material outlineMat;

    private System.Guid id;
    private ITable table;
    private IHand hand;
    private State state;
    private int tableIndex;

    public enum State {
        Offline,
        Selectable,
        Selected,
        Ready,
    };

    void Awake()
    {
        id = System.Guid.NewGuid();
    }

    // Start is called before the first frame update
    void Start()
    {
        outlineMat = gameObject.GetComponent<MeshRenderer>().material;
        table = GameData.Instance.TableObject.GetComponent<ITable>();
        hand = GameData.Instance.HandObject.GetComponent<IHand>();
        if (table == null)
        {
            Debug.Log("Could not find ITable component");
        }
        if (hand == null)
        {
            Debug.Log("Could not find IHand component");
        }
        outlineMat.SetFloat("_CustomAlpha", .6f);
        UpdateColor();
        //state = State.Offline;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateColor();
    }

    public void HandleClick()
    {
        if (state == State.Offline)
            return;

        switch (GameData.Instance.CurrentStage)
        {
            case Stage.Pick:
                table.TryToggleSelect(id);
                break;
            case Stage.Place:
                hand.TryToggleSelect(id);
                break;
        }
    }

    public System.Guid GetId()
    {
        return id;
    }

    public void OnMouseEnter()
    {
        if (state != State.Offline)
            outlineMat.SetFloat("_CustomAlpha", 1f);
    }

    public void OnMouseExit()
    {
        if (state != State.Offline)
            outlineMat.SetFloat("_CustomAlpha", .6f);
    }

    private void UpdateColor()
    {
        switch (state)
        {
            case State.Offline:
                outlineMat.SetColor("_Color", offlineColor);
                break;
            case State.Selectable:
                outlineMat.SetColor("_Color", unselectedColor);
                break;
            case State.Selected:
                outlineMat.SetColor("_Color", selectedColor);
                break;
            case State.Ready:
                outlineMat.SetColor("_Color", readyToSubmitColor);
                break;
        }
    }

    public void FlyToHand(int index)
    {
        state = State.Offline;
        Transform handTransform = GameData.Instance.HandObject.transform;
        transform.SetParent(handTransform);
        StartCoroutine(FlyAnimation(handTransform.position + new Vector3((index - 1) * 1.7f, 0, 0), handTransform.eulerAngles));
    }

    private IEnumerator FlyAnimation(Vector3 pos, Vector3 rot)
    {
        
        float customAnimationTime = 0f;
        Vector3 startPos = transform.position;
        Vector3 startRot = transform.eulerAngles;
        startRot = new Vector3((startRot.x % 360) > 180 ? (startRot.x % 360) - 360 : (startRot.x % 360), startRot.y, startRot.z);
        //Debug.Log($"Start fly animation from {startPos}/{startRot} to {pos}/{rot}");
        while (customAnimationTime <= transitionTime)
        {
            customAnimationTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, pos, transitionCurve.Evaluate(customAnimationTime / transitionTime));
            transform.eulerAngles = Vector3.Lerp(startRot, rot, transitionCurve.Evaluate(customAnimationTime / transitionTime));
            yield return null;
        }
        state = State.Selectable;
    }

    Card.State ICard.CardState
    {
        get { return state; }
        set { state = value; }
    }

    public void Disappear()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null)
            mr.enabled = false;
    }

    public void Delete()
    {
        Destroy(this.gameObject);
    }

    public GameObject GetSpawnableUnit()
    {
        return SpawnedUnit;
    }

    public int TIndex
    {
        get { return tableIndex; }
        set { tableIndex = value; }
    }
}
