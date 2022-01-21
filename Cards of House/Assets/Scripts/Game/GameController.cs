using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : GameData
{
    public GameObject boardObject;
    public GameObject tableObject;
    [SerializeField]
    private GameObject handObject;
    public GameObject cameraTarget;
    [SerializeField]
    public GameObject testUnit;

    private IBoard board;
    private ITable table;
    private IHand hand;
    private ICameraController cam;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        board = boardObject.GetComponent<IBoard>();
        table = tableObject.GetComponent<ITable>();
        hand = handObject.GetComponent<IHand>();
        cam = cameraTarget.GetComponent<ICameraController>();
        cam.AddShot("Board", boardObject.transform, "Board");
        cam.AddShot("Table", tableObject.transform, "Table");
        cam.AddShot("Hand", board.GetSpawnCenterTransform(), "Spawn_0");
        cam.AddShot("HandExpanded", board.GetSpawnCenterTransform(), "Spawn_1");

        board.SetCam(cam);

        cam.TransitionTo(defaultCameras[stage]);
        Debug.Log($"Initial stage: {stage.ToString()}");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Cancel"))
        {
            Debug.Log("Escape pressed");
            Stop();
        }

        if (stage == Stage.Simulate && Input.GetKeyDown(KeyCode.RightArrow))
        {
            board.Step();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            board.SpawnUnit(testUnit, new Vector3Int(0, 0, 0));
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            cam.TransitionTo(defaultCameras[Stage.Place]);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            cam.TransitionTo(defaultCameras[Stage.Simulate]);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            cam.TransitionTo(defaultCameras[Stage.Place]);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            cam.TransitionTo("HandExpanded");
        }

        stageText.text = $"Stage: {stage.ToString()}";
    }

    void FixedUpdate()
    {
        switch (stage)
        {
            case Stage.Pick:
                PickRoutine();
                break;
            case Stage.Place:
                PlaceRoutine();
                break;
            case Stage.Simulate:
                SimulationRoutine();
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
        if (table.Ready())
        {
            stage = Stage.Place;
            hand.Initialize();
            cam.TransitionTo(defaultCameras[Stage.Place]);
        }
    }

    private void PlaceRoutine()
    {
        if (hand.Ready())
        {
            stage = Stage.Simulate;
            board.Initialize();
            cam.TransitionTo(defaultCameras[Stage.Simulate]);
        }
    }

    private void SimulationRoutine()
    {
        if (board.Ready())
        {
            stage = Stage.Pick;
            table.Initialize();
            cam.TransitionTo(defaultCameras[Stage.Pick]);
        }
    }
}
