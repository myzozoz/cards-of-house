using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour, ICameraController
{
    [SerializeField]
    private AnimationCurve transitionCurve;
    [SerializeField]
    private float transitionTime = 1.0f;
    public string shotName;
    [SerializeField]
    public Transform targetTransform;
    public string shotDataPath = "ShotData.json";

    private Shot targetShot;
    private Shot animStartShot;
    private float animTime;

    private Dictionary<string, Shot> shots;
    [SerializeField]
    private Camera cam;

    void Start()
    {
        animTime = transitionTime + 1f;
        shots = LoadShotsFromFile();
        cam = Camera.main;
    }

    void LateUpdate()
    {
        //Debug.Log($"Target: {targetShot.Name}");
        
        //Debug.Log($"Target: {targetShot.Name} Progress: {animTime / transitionTime} ({transitionCurve.Evaluate(animTime / transitionTime)}) ({transform.position})");

    }

    private IEnumerator MoveCamera()
    {
        while (animTime <= transitionTime)
        {
            animTime += Time.deltaTime;
            transform.position = Vector3.Lerp(animStartShot.TargetTransform.position, targetShot.TargetTransform.position, transitionCurve.Evaluate(animTime / transitionTime));
            cam.transform.eulerAngles = Vector3.Lerp(animStartShot.CameraRotation, targetShot.CameraRotation, transitionCurve.Evaluate(animTime / transitionTime));
            cam.transform.position = Vector3.Lerp(animStartShot.CameraOffset, targetShot.TargetTransform.position + targetShot.CameraOffset, transitionCurve.Evaluate(animTime / transitionTime));
            yield return null;
        }
    }

    public void AddShot(string targetName, Transform targetTransform, Vector3 cameraRotation, Vector3 cameraOffset)
    {
        shots[targetName] = new Shot(targetName, targetTransform, cameraRotation, cameraOffset);
    }

    public void AddShot(string targetName, Transform targetTransform, string baseShot)
    {
        if (!shots.ContainsKey(baseShot))
        {
            Debug.Log($"Tried to reference shot '{baseShot}', but it could not be found!");
            return;
        }
        shots[targetName] = new Shot(targetName, targetTransform, shots[baseShot].CameraRotation, shots[baseShot].CameraOffset);
    }

    public bool HasShot(string shotName)
    {
        return shots.ContainsKey(shotName);
    }

    public void TransitionTo(string shot)
    {
        if (!shots.ContainsKey(shot))
        {
            Debug.Log("Could not find shot " + shot);
            return;
        }

        animStartShot = new Shot("Current", transform, cam.transform.eulerAngles, cam.transform.position);
        targetShot = shots[shot];
        animTime = 0;

        StartCoroutine(MoveCamera());
    }

    public void TransitionToFollow(Transform targetObject, string shot)
    {
        if (!shots.ContainsKey(shot))
        {
            Debug.Log("Could not find shot " + shot);
            return;
        }

        Debug.Log($"Moving to follow {targetObject} with shot {shots[shot].Name}");
        //animStartShot = new Shot("Current", transform, cam.transform.eulerAngles, cam.transform.position);

    }

    private Dictionary<string, Shot> LoadShotsFromFile()
    {
        Dictionary<string, Shot> shotDict = new Dictionary<string, Shot>();
        string filePath = Application.persistentDataPath + "/" + shotDataPath;
        ShotData sd = System.IO.File.Exists(filePath) ? JsonUtility.FromJson<ShotData>(System.IO.File.ReadAllText(filePath)) : new ShotData(new List<Shot>());
        foreach (Shot s in sd.Shots)
        {
            shotDict.Add(s.Name, s);
        }

        return shotDict;
    }

    [ContextMenu("Save configuration as JSON")]
    public void SaveConfig()
    {
        Dictionary<string, Shot> shotDict = LoadShotsFromFile();
        Shot newShot = new Shot(shotName, targetTransform, cam.transform.eulerAngles, cam.transform.position - transform.position);
        shotDict[newShot.Name] = newShot;

        ShotData sd = new ShotData(new List<Shot>(shotDict.Values));
        
        //Debug.Log($"{sd.Shots.Count}");
        string json = JsonUtility.ToJson(sd,true);
        //Debug.Log($"Saving {json}");
        System.IO.File.WriteAllText(Application.persistentDataPath + "/ShotData.json", json);
    }
}
