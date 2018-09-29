using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxDistanceJointContractor : MonoBehaviour
{
    [SerializeField]
    MaxDistanceJoint Joint;

    float defaultLength;

    [SerializeField]
    float contractedMaxDistance = 1F;

    // Use this for initialization
    void Start()
    {
        defaultLength = Joint.MaxDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("PullRope"))
        {
            Joint.MaxDistance = contractedMaxDistance;
        }
        else
        {
            Joint.MaxDistance = defaultLength;
        }
    }
}
