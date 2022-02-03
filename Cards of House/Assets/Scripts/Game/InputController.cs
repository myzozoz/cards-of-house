using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private IBoard board;
    private ITable table;
    // Start is called before the first frame update
    void Start()
    {
        board = GameData.Instance.BoardObject.GetComponent<IBoard>();
        table = GameData.Instance.TableObject.GetComponent<ITable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                //Debug.Log($"Ray cast from screen hit {objectHit}");

                IClickable c = objectHit.GetComponent<IClickable>();
                if (c != null)
                {
                    c.HandleClick();
                }
            }
        }

        //PICK STAGE INPUTS
        if (GameData.Instance.CurrentStage == Stage.Pick)
        {
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                //Choose top left card (0)
                table.SendInputToCard(0);
            }
            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                //Choose top right card (1)
                table.SendInputToCard(1);
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                //Choose mid left card (2)
                table.SendInputToCard(2);
            }
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                //Choose center card (3)
                table.SendInputToCard(3);
            }
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                //Choose mid right card (4)
                table.SendInputToCard(4);
            }
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                //Choose bottom left card (5)
                table.SendInputToCard(5);
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                //Choose bottom right card (6)
                table.SendInputToCard(6);
            }
            if (table.ReadyToSubmit() && (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)))
            {
                table.SubmitCards();
            }
        }

        //PLACEMENT STAGE INPUTS
        if (GameData.Instance.CurrentStage == Stage.Place)
        {
            
        }

        //SIMULATION STAGE INPUTS
        if (GameData.Instance.CurrentStage == Stage.Simulate)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //Set target to Avatar
                Debug.Log("NO IMPLEMENTATION YET: Set target to Avatar");
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                //Set target to Unit
                Debug.Log("NO IMPLEMENTATION YET: Set target to Unit");
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                //Set target to Powerup
                Debug.Log("NO IMPLEMENTATION YET: Set target to Powerup");
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                //Perform action Move
                board.Simulate(Command.Move);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                //Perform action Move
                board.Simulate(Command.Wait);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                //Perform action Move
                board.Simulate(Command.Attack);
            }
        }
    }
}
