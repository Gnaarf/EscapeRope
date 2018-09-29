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

        offsetToCamera = CameraAnchors.Length == 0 ? transform.position : transform.position - CameraAnchors[0].position;
    }

    private void Update()
    {
        SetCameraMovement();
    }

    void SetCameraMovement()
    {
        AnchorPoint = Vector3.zero;
        foreach(Transform transform in CameraAnchors)
        {
            AnchorPoint += transform.position;
        }
        AnchorPoint /= CameraAnchors.Length;

        MainCamera.transform.position = AnchorPoint + offsetToCamera;
    }
}
