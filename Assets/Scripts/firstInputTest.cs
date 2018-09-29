using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class firstInputTest : MonoBehaviour {


    public struct ContactPointInfo
    {
        public
        Vector3 hitPoint;
        public
        Vector3 surfaceNormal;
        public 
        float hitTime;
    }

    bool hastBeenUsed = false;
    bool airing = true;
    ContactPointInfo hit;

    ArrayList collidingWith = new ArrayList();

    Rigidbody rb;

    GameObject cameraMain;
    Vector3 offsetToCamera;

	// Use this for initialization
	void Start () {
        rb = this.gameObject.GetComponent<Rigidbody>() ?? null;
        cameraMain = FindObjectOfType<Camera>().gameObject;
        offsetToCamera = cameraMain.transform.position - this.gameObject.transform.position;
        if (rb == null)
        {
            rb = this.gameObject.AddComponent<Rigidbody>();
        }

        Physics.gravity *= 2f;
	}
	
	// Update is called once per frame
	void Update () {
        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        
        if (collidingWith.Count == 0) airing = true;
        else airing = false;

        if (( Mathf.Abs(vertical)+ Mathf.Abs(horizontal) ) >0.5f)
        {
            //Debug.Log(Mathf.Atan2(vertical, horizontal));

            if (!hastBeenUsed) {
                FireEvent(hit, horizontal, vertical);

            } else if (airing)
            {
                float airborneFoce = 50f;
                //rb.AddForce( AirBoring(horizontal, vertical) * airborneFoce);
                rb.AddTorque(AirBoring(horizontal, vertical) * airborneFoce);
            }

            hastBeenUsed = true;
        }
        else
        {
            hastBeenUsed = false;
        }


        //Debug.DrawRay(hit.hitPoint, hit.surfaceNormal * 10f);   
	}

    private void OnCollisionEnter(Collision collision)
    {
        hit.hitPoint = collision.contacts[0].point;
        hit.hitTime = Time.timeSinceLevelLoad;
        hit.surfaceNormal = collision.contacts[0].normal;

        collidingWith.Add( collision.collider);
    }
    private void OnCollisionExit(Collision collision)
    {
        collidingWith.Remove(collision.collider);
    }
    private void FireEvent(ContactPointInfo info, float horizontal, float vertical)
    {
        if (airing) return;
        float timeAllowed = 1.2f;
        float timePassed = info.hitTime - Time.timeSinceLevelLoad;
        float x = 1.0f - Mathf.Sqrt(Mathf.Clamp01(timePassed / timeAllowed));
        float forceAmount = 800f;
        forceAmount += x * forceAmount;
        Vector3 direction = Direction(hit.surfaceNormal, horizontal, vertical);
        if (timePassed < 3f)
        {
            rb.AddForce(direction * forceAmount);

        } else
        {
            RaycastHit hitinfo;
            Physics.Raycast(this.gameObject.transform.position, hit.surfaceNormal * -10f, out hitinfo);
            if(hitinfo.collider != null)
            {
                hit.hitPoint = hitinfo.point;
                hit.hitTime = Time.timeSinceLevelLoad;
                hit.surfaceNormal = hitinfo.normal;
            }
        }


        bool debug = false;

        if(debug)
        Debug.DrawRay(hit.hitPoint, direction * 10f);




    }

    
    Vector3 Direction(Vector3 normal, float hori, float verti)
    {

        Vector3 toConstruct =  new Vector3(Mathf.Abs(normal.x) , Mathf.Abs(normal.y), Mathf.Abs(normal.z));
        Vector3 toReturn = Vector3.Cross(toConstruct, Vector3.up);
        if (toReturn.magnitude == 0f) toReturn = Vector3.Cross(toConstruct, cameraMain.transform.right);
        float angle = Mathf.Atan2(verti, hori);
        //Debug.Log(angle);
        toReturn = Quaternion.AngleAxis(((-1f * angle) * Mathf.Rad2Deg) - 90f , normal) * toReturn;
        toReturn = Vector3.Lerp(normal, toReturn, 0.5f);
        
        return toReturn;
    }

    Vector3 AirBoring(float hori, float verti)
    {
        Vector3 toReturn = Vector3.Cross(Vector3.up, cameraMain.transform.right);
        float angle = Mathf.Atan2(verti, hori);
        Debug.Log(angle);
        toReturn = Quaternion.AngleAxis(((-1f * angle) * Mathf.Rad2Deg) - 90f, Vector3.up) * toReturn;

        return toReturn;
    }
}
