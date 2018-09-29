using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MaxDistanceJointColorizer : MonoBehaviour
{
    [SerializeField]
    MaxDistanceJoint JointHolder;

    [SerializeField]
    Gradient Gradient;

    LineRenderer LineRenderer;

    // Use this for initialization
    void Start()
    {
        LineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float t = Mathf.Clamp01(JointHolder.GetCurrentDistance() / JointHolder.MaxDistance);
        //LineRenderer.material.color = Gradient.Evaluate(t);
        LineRenderer.material.SetColor("_EmissionColor", Gradient.Evaluate(t));

        LineRenderer.SetPositions(new Vector3[] { JointHolder.transform.position, JointHolder.ConnectedBody.transform.position });
    }
}
