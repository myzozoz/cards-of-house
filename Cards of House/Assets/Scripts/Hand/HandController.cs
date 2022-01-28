using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandController : MonoBehaviour, IHand
{
    [SerializeField]
    private Button submitButton;
    private bool ready;
    private Dictionary<System.Guid, ICard> cards = new Dictionary<System.Guid, ICard>();
    private List<System.Guid> selectedCards = new List<System.Guid>();
    private HashSet<System.Guid> lockedCards = new HashSet<System.Guid>();
    private IBoard board;
    private enum HandSubStage
    {
        ChooseCard,
        ChooseLocation,
        ReadyToSubmit
    };
    private HandSubStage substage;

    // Start is called before the first frame update
    void Start()
    {
        board = GameData.Instance.BoardObject.GetComponent<IBoard>();
        submitButton.gameObject.SetActive(false);
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
        selectedCards.Clear();
        cards.Clear();
        lockedCards.Clear();
        List<ICard> cardList = GameData.Instance.HandCards;
        ready = false;
        substage = HandSubStage.ChooseCard;

        for (int i = 0; i < cardList.Count; i++)
        {
            cards[cardList[i].GetId()] = cardList[i];
            cardList[i].FlyToHand(i);
        }
        submitButton.gameObject.SetActive(true);
        submitButton.interactable = false;
        board.UpdateSpawns();
    }

    public void End()
    {
        //clean up
        submitButton.gameObject.SetActive(false);
        foreach (System.Guid id in cards.Keys)
        {
            //Do fancy disappearing effect
            cards[id].Disappear();
        }
        cards.Clear();
    }

    public void TryToggleSelect(System.Guid id)
    {
        if (lockedCards.Contains(id))
        {
            //Debug.Log($"Going to locked cards function");
            lockedCards.Remove(id);
            if (selectedCards.Count > 0)
            {
                cards[selectedCards[0]].CardState = Card.State.Selectable;
                selectedCards.Clear();
            }
            cards[id].CardState = Card.State.Selected;
            selectedCards.Clear();
            selectedCards.Add(id);
            board.EnableSpawnSelection();
            //Debug.Log($"{cards[selectedCards[0]].IsSelected()} / {cards[selectedCards[0]].Selectable}");
            substage = HandSubStage.ChooseCard;
        }
        else if (selectedCards.Count > 0 && selectedCards[0] == id)
        {
            selectedCards.Clear();
            substage = HandSubStage.ChooseCard;
            board.DisableSpawnSelection();
            cards[id].CardState = Card.State.Selectable;
        }
        else
        {
            if (selectedCards.Count > 0)
            {
                cards[selectedCards[0]].CardState = Card.State.Selectable;
                selectedCards.Clear();
            }
            selectedCards.Add(id);
            substage = HandSubStage.ChooseLocation;
            board.EnableSpawnSelection();
            cards[id].CardState = Card.State.Selected;
        }

        UpdateReadyStatus();
        //Debug.Log($"Selected cards: {selectedCards.Count}");
    }

    private void UpdateReadyStatus()
    {
        if (lockedCards.Count >= cards.Count)
        {
            substage = HandSubStage.ReadyToSubmit;
            submitButton.interactable = true;
        } else
        {
            submitButton.interactable = false;
        }
    }

    public bool ReadyToSubmit()
    {
        return substage == HandSubStage.ReadyToSubmit;
    }

    public ICard GetSelectedCard()
    {
        return cards[selectedCards[0]];    
    }

    public void LockSelectedCard()
    {
        System.Guid selected = selectedCards[0];
        selectedCards.Clear();
        lockedCards.Add(selected);
        if (lockedCards.Count == cards.Count)
        {
            substage = HandSubStage.ReadyToSubmit;
        }
        cards[selected].CardState = Card.State.Ready;
        //Debug.Log($"Card locked. {cards[selected].IsSelected()} / {cards[selected].Selectable}");
        board.DisableSpawnSelection();

        UpdateReadyStatus();
    }

    public void SubmitSpawns()
    {
        Debug.Log($"Submitting spawn cards {cards.Count}");
        ready = true;
    }
}
