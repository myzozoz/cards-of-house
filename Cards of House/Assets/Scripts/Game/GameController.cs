using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject boardObject;


    private IBoard board;

    // Start is called before the first frame update
    void Start()
    {
        board = boardObject.GetComponent<IBoard>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Cancel"))
        {
            Debug.Log("Escape pressed");
            Stop();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            board.Step();
        }
    }

    private void Stop()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
