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

    private Dictionary<System.Guid, ICard> cards;
    private HashSet<System.Guid> selectedCards;
    private bool canSubmit;
    private bool ready;

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
        cards = new Dictionary<System.Guid, ICard>();
        selectedCards = new HashSet<System.Guid>();
        List<ICard> cardList = new List<ICard>(GetComponentsInChildren<ICard>());
        foreach (ICard c in cardList)
        {
            cards.Add(c.GetId(), c);
            c.CardState = Card.State.Selectable;
        }
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
        }

        GameData.Instance.HandCards = hand;
        ready = true;
    }

    public bool Ready()
    {
        return this.ready;
    }
}
