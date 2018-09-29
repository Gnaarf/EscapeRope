using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerDeformOnSpeed : MonoBehaviour {
    Rigidbody rb;
    public Material playerMat;
    RaycastHit[] hitInfos;
    Vector3 currentVelocity;
    Vector3 rayOrigin;
    Vector3 RayDirection;

    Vector3 currentFront;

    float offeset;

	// Use this for initialization
	void Start () {
        rb = this.gameObject.GetComponent<Rigidbody>();
        Vector3 extend = this.GetComponent<MeshFilter>().mesh.bounds.extents;
        offeset = Mathf.Max(Mathf.Max(extend.x, extend.y), extend.z) *2f;


    }
	
	// Update is called once per frame
	void Update () {

        currentVelocity = rb.velocity;

        if (rb.velocity.magnitude > 0f)
        {
            rayOrigin = this.gameObject.transform.position + currentVelocity.normalized * offeset;
            RayDirection = -currentVelocity.normalized;


            hitInfos = Physics.RaycastAll(rayOrigin, RayDirection, offeset * 2f);
            foreach(RaycastHit hitinfo in hitInfos)
            {
                if (hitinfo.collider == null) continue;
                if (hitinfo.collider.gameObject != this.gameObject) continue;

                currentFront = hitinfo.point;
                break;
            }

            Debug.DrawRay(currentFront, currentVelocity);
        }

        playerMat.SetVector("_FrontPoint", currentFront);
        playerMat.SetVector("_Velocity", currentVelocity);


    }
}
