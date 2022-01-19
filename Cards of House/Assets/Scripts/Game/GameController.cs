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

    private Vector3 boardCamRotation = new Vector3(60f, 0f, 0f);
    private Vector3 boardCamOffset = new Vector3(2.5f, 15f, -4.3f);
    private Vector3 tableCamRotation = new Vector3(45f, 0f, 0f);
    private Vector3 tableCamOffset = new Vector3(0f, 6.6f, -6.7f);

    // Start is called before the first frame update
    void Start()
    {
        board = boardObject.GetComponent<IBoard>();
        cam = cameraTarget.GetComponent<ICameraController>();
        stageText.text = $"Stage: {stage.ToString()}";
        Debug.Log($"TableObject position: {tableObject.transform.position}");
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
