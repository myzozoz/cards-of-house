using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject boardObject;
    public GameObject tableObject;
    public GameObject cameraTarget;
    [SerializeField]
    public GameObject testUnit;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private Vector3 PickCameraPoint;
    [SerializeField]
    private Vector3 SimulationCameraPoint;

    //For debugging
    [SerializeField]
    private Text stageText;
    [SerializeField]
    private Stage stage;

    private IBoard board;
    private ICameraController cam;

    // Start is called before the first frame update
    void Start()
    {
        board = boardObject.GetComponent<IBoard>();
        cam = cameraTarget.GetComponent<ICameraController>();
        stageText.text = $"Stage: {stage.ToString()}";
        cam.AddShot("Board", boardObject.transform, "Board");
        cam.AddShot("Table", tableObject.transform, "Table");

        board.SetCam(cam);
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

        if (Input.GetKeyDown(KeyCode.S))
        {
            board.SpawnUnit(testUnit, new Vector3Int(0, 0, 0));
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            cam.TransitionTo("Table");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            cam.TransitionTo("Board");
        }
    }

    void FixedUpdate()
    {
        switch (stage)
        {
            case Stage.Pick:
                PickRoutine();
                break;
            case Stage.Place:
                break;
            case Stage.Simulate:
                break;
            default:
                break;
        }
    }

    private void Stop()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void PickRoutine()
    {
        
    }

    private void PlaceRoutine()
    {

    }

    private void SimulationRoutine()
    {

    }
}
