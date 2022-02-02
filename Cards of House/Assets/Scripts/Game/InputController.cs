using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private IBoard board;
    // Start is called before the first frame update
    void Start()
    {
        board = GameData.Instance.BoardObject.GetComponent<IBoard>();
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
