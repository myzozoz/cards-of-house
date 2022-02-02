using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : GenericSingleton<GameController>
{
    public GameObject cameraTarget;
    [SerializeField]
    public GameObject testUnit;
    [SerializeField]
    private Text stageText;
    [SerializeField]
    private Text roundText;
    [SerializeField]
    private float simulationSpeed = 1f;

    private IBoard board;
    private ITable table;
    private IHand hand;
    private ICameraController cam;


    // Start is called before the first frame update
    void Start()
    {
        board = GameData.Instance.BoardObject.GetComponent<IBoard>();
        table = GameData.Instance.TableObject.GetComponent<ITable>();
        hand = GameData.Instance.HandObject.GetComponent<IHand>();

        cam = cameraTarget.GetComponent<ICameraController>();
        cam.AddShot("Board", GameData.Instance.BoardObject.transform, "Board");
        cam.AddShot("Table", GameData.Instance.TableObject.transform, "Table");
        cam.AddShot("Hand", board.GetSpawnCenterTransform(), "Spawn_0");
        cam.AddShot("HandExpanded", board.GetSpawnCenterTransform(), "Spawn_1");
        board.SetCam(cam);
        //Debug.Log($"Debug: {GameData.Instance.DefaultCameras}");
        cam.TransitionTo(GameData.Instance.DefaultCameras[GameData.Instance.CurrentStage]);
        //Debug.Log($"Initial stage: {GameData.Instance.CurrentStage.ToString()}");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Cancel"))
        {
            Debug.Log("Escape pressed");
            Stop();
        }

        if (GameData.Instance.CurrentStage == Stage.Simulate && Input.GetKeyDown(KeyCode.RightArrow))
        {
            //board.SimulateRound();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            board.SpawnUnit(testUnit, new Vector3Int(0, 0, 0));
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            cam.TransitionTo(GameData.Instance.DefaultCameras[Stage.Place]);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            cam.TransitionTo(GameData.Instance.DefaultCameras[Stage.Simulate]);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            cam.TransitionTo(GameData.Instance.DefaultCameras[Stage.Place]);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            cam.TransitionTo("HandExpanded");
        }

        stageText.text = $"Stage: {GameData.Instance.CurrentStage.ToString()}";
        roundText.text = GameData.Instance.CurrentStage == Stage.Simulate ? $"Round: {board.Round}" : "";

        if (GameData.Instance.WState == WinState.Won)
        {
            StopAllCoroutines();
            board.End();
            StartCoroutine(WinRoutine());
            GameData.Instance.WState = WinState.End;
        }
        else if (GameData.Instance.WState == WinState.Lost)
        {
            StopAllCoroutines();
            board.End();
            StartCoroutine(LoseRoutine());
            GameData.Instance.WState = WinState.End;
        }
    }

    void FixedUpdate()
    {
        switch (GameData.Instance.CurrentStage)
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
            table.End();
            GameData.Instance.CurrentStage = Stage.Place;
            hand.Initialize();
            cam.TransitionTo(GameData.Instance.DefaultCameras[Stage.Place]);
        }
    }

    private void PlaceRoutine()
    {
        if (hand.Ready())
        {
            hand.End();
            GameData.Instance.CurrentStage = Stage.Simulate;
            board.Initialize();
            cam.TransitionTo(GameData.Instance.DefaultCameras[Stage.Simulate]);
        }
    }

    private void SimulationRoutine()
    {
        if (board.Ready())
        {
            board.End();
            GameData.Instance.CurrentStage = Stage.Pick;
            table.Initialize();
            cam.TransitionTo(GameData.Instance.DefaultCameras[Stage.Pick]);
        }
    }

    void OnValidate()
    {
        Time.timeScale = simulationSpeed;
    }
    
    private IEnumerator WinRoutine()
    {
        Debug.Log("We win! :party:");
        yield return new WaitForSeconds(2f);
        Stop();
    }

    private IEnumerator LoseRoutine()
    {
        Debug.Log("We lost! :(");
        yield return new WaitForSeconds(2f);
        Stop();
    }
}
