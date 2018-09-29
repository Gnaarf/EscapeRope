using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeGenerator : MonoBehaviour
{
    [SerializeField]
    private Rigidbody StartConnectedBody;

    [SerializeField]
    private Rigidbody EndConnectedBody;

    [SerializeField]
    private Joint RopeSegment;

    [SerializeField]
    private int SegmentCount = 10;

    List<Joint> RopeSegments = new List<Joint>();

    private void Start()
    {
        GenerateRope();
    }

    void GenerateRope()
    {
        foreach(Joint segment in RopeSegments)
        {
            Object.Destroy(segment.gameObject);
        }
        RopeSegments.Clear();

        Joint previousSegment = null;
        Joint currentSegment = null;

        for (int i = 1; i < SegmentCount - 1; ++i)
        {
            currentSegment = Instantiate(RopeSegment, GetRopeSegmentPosition(i), Quaternion.identity);
            currentSegment.transform.parent = this.transform;

            RopeSegments.Add(currentSegment);

            currentSegment.gameObject.SetActive(true);

            if (previousSegment != null)
            {
                previousSegment.connectedBody = currentSegment.GetComponent<Rigidbody>();
            }

            previousSegment = currentSegment;
        }

        Destroy(currentSegment);

        if(StartConnectedBody != null)
        {
            FixedJoint fixedJoint = RopeSegments[0].gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = StartConnectedBody;
        }
        if(EndConnectedBody != null)
        {
            FixedJoint fixedJoint = RopeSegments[RopeSegments.Count - 1].gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = EndConnectedBody;
        }
    }

    private Vector3 GetRopeSegmentPosition(int RopeSegmentIndex)
    {
        Vector3 startPosition = StartConnectedBody == null ? Vector3.zero : StartConnectedBody.transform.position;
        Vector3 endPosition = EndConnectedBody == null ? Vector3.one : EndConnectedBody.transform.position;
        return Vector3.Lerp(startPosition, endPosition, RopeSegmentIndex / (float)(SegmentCount - 1));
    }

    private void OnDrawGizmos()
    {
        if (RopeSegment != null)
        {
            for (int i = 1; i < SegmentCount - 1; ++i)
            {
                Gizmos.DrawWireSphere(GetRopeSegmentPosition(i), RopeSegment.transform.lossyScale.x / 3F);
            }
        }
    }
}
