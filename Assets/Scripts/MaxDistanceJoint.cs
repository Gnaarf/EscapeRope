using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxDistanceJoint : MonoBehaviour
{

    [SerializeField]
    Rigidbody _connectedBody;

    public Rigidbody ConnectedBody { get { return _connectedBody; } }

    [SerializeField]
    float _maxDistance = 10F;

    public float MaxDistance { get { return _maxDistance; } }

    [SerializeField]
    float forceIntensity = 10F;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float distance = GetCurrentDistance();
        if (distance >= _maxDistance)
        {
            Vector3 ownPosition = transform.position;
            Vector3 otherPosition = _connectedBody.transform.position;

            Vector3 forceVector = (otherPosition - ownPosition) * forceIntensity;

            GetComponent<Rigidbody>().AddForce(forceVector);
            _connectedBody.GetComponent<Rigidbody>().AddForce(-forceVector);
        }
    }

    public float GetCurrentDistance()
    {
        return Vector3.Distance(this.transform.position, _connectedBody.transform.position);
    }
}
