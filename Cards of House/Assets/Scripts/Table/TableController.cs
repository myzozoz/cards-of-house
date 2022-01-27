using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableController : MonoBehaviour, ITable
{
    [SerializeField]
    private short maxSelections = 3;
    [SerializeField]
    private Button submitButton;
    [SerializeField]
    private GameObject cardObject;

    private Dictionary<System.Guid, ICard> cards = new Dictionary<System.Guid, ICard>();
    private HashSet<System.Guid> selectedCards = new HashSet<System.Guid>();
    private bool canSubmit;
    private bool ready;

    private static List<Vector3> cardPositions = new List<Vector3>() {
        new Vector3(-1.58f,0f,2.318f),
        new Vector3(1.58f,0f,2.318f),
        new Vector3(-1.58f,0f,0f),
        new Vector3(0f,0f,0f),
        new Vector3(1.58f,0f,0f),
        new Vector3(-1.58f,0f,-2.318f),
        new Vector3(1.58f,0f,-2.318f),
    };

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    public void End()
    {
        //do we need to stop handling some events? :shrug:
    }

    //Draw new cards and other fancy stuff here
    public void Initialize()
    {
        selectedCards.Clear();
        SpawnCards();
        ready = false;
        UpdateCanSubmit();
    }

    public void TryToggleSelect(System.Guid id)
    {
        if (selectedCards.Contains(id))
        {
            if (ReadyToSubmit())
            {
                foreach (System.Guid _id in selectedCards)
                {
                    cards[_id].CardState = Card.State.Selected;
                }
            }
            selectedCards.Remove(id);
            cards[id].CardState = Card.State.Selectable;
            UpdateCanSubmit();
        }
        else if (selectedCards.Count < maxSelections)
        {
            selectedCards.Add(id);
            cards[id].CardState = Card.State.Selected;
            UpdateCanSubmit();
        }
    }

    public bool ReadyToSubmit()
    {
        return canSubmit;
    }

    private void UpdateCanSubmit()
    {
        canSubmit = selectedCards.Count == maxSelections;
        submitButton.interactable = canSubmit;
        if (canSubmit)
        {
            foreach (System.Guid _id in selectedCards)
            {
                cards[_id].CardState = Card.State.Ready;
            }
        }
    }

    public void SubmitCards()
    {
        List<ICard> hand = new List<ICard>();
        foreach (System.Guid id in selectedCards)
        {
            hand.Add(cards[id]);
            cards.Remove(id);
        }

        GameData.Instance.HandCards = hand;
        ready = true;
    }

    public bool Ready()
    {
        return this.ready;
    }

    public void SpawnCards()
    {
        foreach (ICard c in cards.Values)
        {
            if (c != null)
                c.Delete();
        }
        cards.Clear();

        foreach (Vector3 v in cardPositions)
        {
            GameObject go = Instantiate(cardObject, this.transform, false);
            go.transform.Translate(v);
            ICard c = go.GetComponent<ICard>();
            cards.Add(c.GetId(), c);
            c.CardState = Card.State.Selectable;
            //Debug.Log($"Card {c.GetId()} state: {c.CardState.ToString()}");
        }
    }
}
