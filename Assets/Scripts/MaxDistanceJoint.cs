using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxDistanceJoint : MonoBehaviour
{

    [SerializeField]
    Rigidbody ConnectedBody;

    [SerializeField]
    float MaxDistance = 10F;

    [SerializeField]
    float forceIntensity = 10F;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(this.transform.position, ConnectedBody.transform.position);
        if (distance >= MaxDistance)
        {
            Vector3 ownPosition = transform.position;
            Vector3 otherPosition = ConnectedBody.transform.position;

            Vector3 forceVector = (otherPosition - ownPosition) * forceIntensity;

            GetComponent<Rigidbody>().AddForce(forceVector);
            ConnectedBody.GetComponent<Rigidbody>().AddForce(-forceVector);
        }
    }
}
