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

        for (int i = 0; i < SegmentCount; ++i)
        {
            currentSegment = Instantiate(RopeSegment, this.transform);

            RopeSegments.Add(currentSegment);

            currentSegment.gameObject.SetActive(true);

            if (previousSegment != null)
            {
                previousSegment.connectedBody = currentSegment.GetComponent<Rigidbody>();
            }

            previousSegment = currentSegment;
        }

        ResetRopeSegmentPositions();

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



    private void ResetRopeSegmentPositions()
    {
        for (int i = 0; i < SegmentCount; ++i)
        {
            RopeSegments[i].transform.position = GetRopeSegmentPosition(i);
        }
    }

    private Vector3 GetRopeSegmentPosition(int RopeSegmentIndex)
    {
        Vector3 startPosition = StartConnectedBody == null ? Vector3.zero : StartConnectedBody.transform.position;
        Vector3 endPosition = EndConnectedBody == null ? Vector3.one : EndConnectedBody.transform.position;
        return Vector3.Lerp(startPosition, endPosition, RopeSegmentIndex / (float)SegmentCount);
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < SegmentCount; ++i)
        {
            Gizmos.DrawWireSphere(GetRopeSegmentPosition(i), RopeSegment.transform.lossyScale.x / 3F);
        }
    }
}
