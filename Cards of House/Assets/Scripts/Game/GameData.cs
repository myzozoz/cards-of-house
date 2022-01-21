using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This is a data container for the GameController class, but
//which exposes its data to outside functions
public class GameData : MonoBehaviour
{
    [SerializeField]
    protected Text stageText;
    [SerializeField]
    protected Stage stage;

    protected List<ICard> handCards;

    protected Dictionary<Stage, string> defaultCameras;

    // Start is called before the first frame update
    protected void Start()
    {
        handCards = new List<ICard>();
        defaultCameras = new Dictionary<Stage, string>()
        {
            {Stage.Pick, "Table"},
            {Stage.Place, "Hand"},
            {Stage.Simulate, "Board" }
        };
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
}
