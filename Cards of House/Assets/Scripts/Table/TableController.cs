using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableController : MonoBehaviour, ITable
{
    [SerializeField]
    private short maxSelections = 3;

    private Dictionary<System.Guid, ICard> cards;
    private HashSet<System.Guid> selectedCards;
    // Start is called before the first frame update
    void Start()
    {
        cards = new Dictionary<System.Guid, ICard>();
        selectedCards = new HashSet<System.Guid>();
        List<ICard> cardList = new List<ICard>(GetComponentsInChildren<ICard>());
        foreach (ICard c in cardList)
        {
            cards.Add(c.GetId(), c);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool TryToggleSelect(System.Guid id)
    {
        if (selectedCards.Contains(id))
        {
            selectedCards.Remove(id);
            return true;
        }

        if (selectedCards.Count < maxSelections)
        {
            selectedCards.Add(id);
            return true;
        }

        return false;
    }
}
