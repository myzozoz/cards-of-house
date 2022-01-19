using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour, ICameraController
{
    [SerializeField]
    private AnimationCurve transitionCurve;
    [SerializeField]
    private float transitionTime = 1.0f;

    private Shot targetShot;
    private Shot animStartShot;
    private float animTime;

    private Dictionary<string, Shot> shots;
    private Camera cam;

    void Start()
    {
        animTime = transitionTime + 1f;
        shots = new Dictionary<string, Shot>();
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (animTime > transitionTime)
        {
            return;
        }

        //Debug.Log($"Target: {targetShot.Name}");
        animTime += Time.deltaTime;
        transform.position = Vector3.Lerp(animStartShot.TargetPosition, targetShot.TargetPosition, transitionCurve.Evaluate(animTime / transitionTime));
        cam.transform.eulerAngles = Vector3.Lerp(animStartShot.CameraRotation, targetShot.CameraRotation, transitionCurve.Evaluate(animTime / transitionTime));
        cam.transform.position = Vector3.Lerp(animStartShot.CameraOffset, targetShot.TargetPosition + targetShot.CameraOffset, transitionCurve.Evaluate(animTime / transitionTime));
        //Debug.Log($"Target: {targetShot.Name} Progress: {animTime / transitionTime} ({transitionCurve.Evaluate(animTime / transitionTime)}) ({transform.position})");

    }

    public void AddShot(string targetName, Vector3 targetPosition, Vector3 cameraRotation, Vector3 cameraOffset)
    {
        Shot s = new Shot();
        s.Name = targetName;
        s.TargetPosition = targetPosition;
        s.CameraRotation = cameraRotation;
        s.CameraOffset = cameraOffset;
        shots[targetName] = s;
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

        Shot s = new Shot();
        s.Name = "Current";
        s.TargetPosition = transform.position;
        s.CameraRotation = cam.transform.eulerAngles;
        s.CameraOffset = cam.transform.position;

        animStartShot = s;
        targetShot = shots[shot];
        animTime = 0;
    }
}
