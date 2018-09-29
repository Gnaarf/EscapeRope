using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class secondInputTest : MonoBehaviour
{
    public struct ContactPointInfo
    {
        public
        Vector3 hitPoint;
        public
        Vector3 surfaceNormal;
        public
        float hitTime;
    }

    enum ControlScheme
    {
        Original,
        Simple,
    }

    [SerializeField]
    int playerIndex = 1;

    [SerializeField]
    ControlScheme controlScheme = ControlScheme.Simple;


    bool hastBeenUsed = false;
    bool airing = true;
    ContactPointInfo hit;

    ArrayList collidingWith = new ArrayList();

    Rigidbody rb;

    GameObject cameraMain;
    Vector3 offsetToCamera;

    float recalculateHitTime = 3F;

    ParticleSystem JumpChargeParticleSystem;

    // Use this for initialization
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>() ?? null;
        cameraMain = FindObjectOfType<Camera>().gameObject;
        offsetToCamera = cameraMain.transform.position - this.gameObject.transform.position;
        if (rb == null)
        {
            rb = this.gameObject.AddComponent<Rigidbody>();
        }

        Physics.gravity *= 2f;

        JumpChargeParticleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal" + playerIndex);
        float vertical = Input.GetAxis("Vertical" + playerIndex);


        if (collidingWith.Count == 0) airing = true;
        else airing = false;

        if ((Mathf.Abs(vertical) + Mathf.Abs(horizontal)) > 0.5f)
        {
            //Debug.Log(Mathf.Atan2(vertical, horizontal));

            if (!hastBeenUsed)
            {
                FireEvent(hit, horizontal, vertical);

            }
            else if (airing)
            {
                float airborneFoce = 50f;
                //rb.AddForce( AirBoring(horizontal, vertical) * airborneFoce);
                //rb.AddTorque(AirBoring(horizontal, vertical) * airborneFoce);
            }

            hastBeenUsed = true;
        }
        else
        {
            hastBeenUsed = false;
        }
        
        if (airing)
        {
            if (JumpChargeParticleSystem.isPlaying)
                JumpChargeParticleSystem.Stop();
        }
        else
        {
            if (!JumpChargeParticleSystem.isPlaying)
                JumpChargeParticleSystem.Play();
        }


        //Debug.DrawRay(hit.hitPoint, hit.surfaceNormal * 10f);   
    }

    private void OnCollisionEnter(Collision collision)
    {
        hit.hitPoint = collision.contacts[0].point;
        hit.hitTime = Time.timeSinceLevelLoad;
        hit.surfaceNormal = collision.contacts[0].normal;

        collidingWith.Add(collision.collider);
    }
    private void OnCollisionExit(Collision collision)
    {
        collidingWith.Remove(collision.collider);
    }
    private void FireEvent(ContactPointInfo info, float horizontal, float vertical)
    {
        if (airing) return;

        switch (controlScheme)
        {
            case ControlScheme.Original:
                PerformOriginalControlScheme(info, horizontal, vertical);
                break;

            case ControlScheme.Simple:
                PerformSimpleControlScheme(info, horizontal, vertical);
                break;
        }

        float timePassed = info.hitTime - Time.timeSinceLevelLoad;
        if (timePassed >= recalculateHitTime)
        {
            RaycastHit hitinfo;
            Physics.Raycast(this.gameObject.transform.position, hit.surfaceNormal * -10f, out hitinfo);
            if (hitinfo.collider != null)
            {
                hit.hitPoint = hitinfo.point;
                hit.hitTime = Time.timeSinceLevelLoad;
                hit.surfaceNormal = hitinfo.normal;
            }
        }

        if (runInEditMode)
        {
            Vector3 direction = OriginalDirection(hit.surfaceNormal, horizontal, vertical);
            Debug.DrawRay(hit.hitPoint, direction * 10f);
        }
    }

    private void PerformOriginalControlScheme(ContactPointInfo info, float horizontal, float vertical)
    {
        float timeAllowed = 1.2f;
        float timePassed = info.hitTime - Time.timeSinceLevelLoad;
        float x = 1.0f - Mathf.Sqrt(Mathf.Clamp01(timePassed / timeAllowed));
        float forceAmount = 800f;
        forceAmount += x * forceAmount;
        Vector3 direction = OriginalDirection(info.surfaceNormal, horizontal, vertical);
        if (timePassed < recalculateHitTime)
        {
            rb.AddForce(direction * forceAmount);

        }
    }

    Vector3 OriginalDirection(Vector3 normal, float hori, float verti)
    {

        Vector3 toConstruct = new Vector3(Mathf.Abs(normal.x), Mathf.Abs(normal.y), Mathf.Abs(normal.z));
        Vector3 toReturn = Vector3.Cross(toConstruct, Vector3.up);
        if (toReturn.magnitude == 0f) toReturn = Vector3.Cross(toConstruct, cameraMain.transform.right);
        float angle = Mathf.Atan2(verti, hori);
        //Debug.Log(angle);
        toReturn = Quaternion.AngleAxis(((-1f * angle) * Mathf.Rad2Deg) - 90f, normal) * toReturn;
        toReturn = Vector3.Lerp(normal, toReturn, 0.5f);

        return toReturn;
    }

    private void PerformSimpleControlScheme(ContactPointInfo info, float horizontal, float vertical)
    {
        float timeAllowed = 1.2f;
        float timePassed = info.hitTime - Time.timeSinceLevelLoad;
        float x = 1.0f - Mathf.Sqrt(Mathf.Clamp01(timePassed / timeAllowed));
        float forceAmount = 800f;
        forceAmount += x * forceAmount;
        Vector3 direction = GetSimpleDirection(info.surfaceNormal, horizontal, vertical);
        if (timePassed < recalculateHitTime)
        {
            rb.AddForce(direction * forceAmount);

        }
    }

    Vector3 GetSimpleDirection(Vector3 normal, float hori, float verti)
    {
        Vector3 forward = Vector3.ProjectOnPlane(cameraMain.transform.forward, Vector3.up);
        Vector3 right = cameraMain.transform.right;
        Vector3 up = Vector3.up;

        Vector3 result = (right * hori + forward * verti + up).normalized;

        float angleBetweenResultAndNormal = Vector3.Angle(result, normal);
        if (angleBetweenResultAndNormal >= 90)
        {
            Vector3 tangent = Vector3.ProjectOnPlane(result, normal);
            result = Vector3.Lerp(tangent, normal, 0.1F).normalized;
        }

        return result;
    }

    Vector3 AirBoring(float hori, float verti)
    {
        Vector3 toReturn = Vector3.Cross(Vector3.up, cameraMain.transform.right);
        float angle = Mathf.Atan2(verti, hori);
        Debug.Log(angle);
        toReturn = Quaternion.AngleAxis(((-1f * angle) * Mathf.Rad2Deg) - 90f, Vector3.up) * toReturn;

        return toReturn;
    }

    private void OnDrawGizmos()
    {
        Color tmp = Gizmos.color;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5);
        Gizmos.color = tmp;
    }
}
