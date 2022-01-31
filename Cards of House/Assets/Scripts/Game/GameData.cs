using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This is a data container for the GameController class, but
//which exposes its data to outside functions
public class GameData : GenericSingleton<GameData>
{
    [SerializeField]
    private GameObject boardObject;
    [SerializeField]
    private GameObject tableObject;
    [SerializeField]
    private GameObject handObject;
    [SerializeField]
    private Stage stage;

    private WinState winState = WinState.Undecided;

    private List<ICard> handCards;
    private Dictionary<Stage, string> defaultCameras = new Dictionary<Stage, string>()
        {
            {Stage.Pick, "Table"},
            {Stage.Place, "Hand"},
            {Stage.Simulate, "Board" }
        };

// Start is called before the first frame update
    protected void Start()
    {
        handCards = new List<ICard>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<ICard> GetHandCards()
    {
        return handCards;
    }

    public void SetHandCards(List<ICard> cards)
    {
        if (cards.Count != 3)
        {
            Debug.Log("Hand size is supposed to be 3 >:(");
        }
        handCards = cards;
    }

    public List<ICard> HandCards
    {
        get { return handCards; }
        set { handCards = value; }
    }

    public Stage CurrentStage
    {
        get { return stage; }
        set { stage = value; }
    }

    public Dictionary<Stage, string> DefaultCameras
    {
        get { return defaultCameras; }
    }

    public GameObject BoardObject
    {
        get { return boardObject; }
    }

    public GameObject TableObject
    {
        get { return tableObject; }
    }

    public GameObject HandObject
    {
        get { return handObject; }
    }

    public WinState WState
    {
        get { return winState; }
        set { winState = value; }
    }
}
