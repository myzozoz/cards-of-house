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
    private GameData data;

    // Start is called before the first frame update
    void Start()
    {
        data = GameObject.FindWithTag("GameController").GetComponent<GameData>();
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
            c.SetSelected(false);
        }
        ready = false;
        UpdateCanSubmit();
    }

    public bool TryToggleSelect(System.Guid id)
    {
        if (selectedCards.Contains(id))
        {
            selectedCards.Remove(id);
            UpdateCanSubmit();
            return true;
        }

        if (selectedCards.Count < maxSelections)
        {
            selectedCards.Add(id);
            UpdateCanSubmit();
            return true;
        }

        return false;
    }

    public bool ReadyToSubmit()
    {
        return canSubmit;
    }

    private void UpdateCanSubmit()
    {
        canSubmit = selectedCards.Count == maxSelections;
        submitButton.interactable = canSubmit;
    }

    public void SubmitCards()
    {
        Debug.Log("Submitting cards");
        List<ICard> hand = new List<ICard>();
        foreach (System.Guid id in selectedCards)
        {
            hand.Add(cards[id]);
        }

        data.SetHandCards(hand);
        ready = true;
        
    }

    public bool Ready()
    {
        return this.ready;
    }
}
