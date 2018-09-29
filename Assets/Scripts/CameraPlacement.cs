using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlacement : MonoBehaviour
{
    [SerializeField]
    Transform[] CameraAnchors = new Transform[0];

    Camera MainCamera;
    Vector3 AnchorPoint;
    Vector3 offsetToCamera;

    void Start()
    {
        MainCamera = Camera.main;

        print(MainCamera);

        if (CameraAnchors.Length > 0)
        {
            offsetToCamera = MainCamera.transform.position - GetAveragePosition();
        }
    }

    private void Update()
    {
        SetCameraMovement();
    }

    void SetCameraMovement()
    {
        AnchorPoint = GetAveragePosition();

        MainCamera.transform.position = AnchorPoint + offsetToCamera;
    }

    Vector3 GetAveragePosition()
    {
        Vector3 result = Vector3.zero;
        foreach (Transform transform in CameraAnchors)
        {
            result += transform.position;
        }
        result /= CameraAnchors.Length;

        return result;
    }
}
