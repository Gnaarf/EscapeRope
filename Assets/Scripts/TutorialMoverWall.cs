using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMoverWall : MonoBehaviour
{
    [SerializeField]
    Transform _target;

    public Vector3 Target { get; private set; }

    public void StartMoving(float animationTime)
    {
        Target = _target.position;

        StartCoroutine(Move(animationTime));

    }

    IEnumerator Move(float animationTime)
    {
        float startTime = Time.time;
        Vector3 startPosition = transform.position;

        float t = 0;

        while (t < 1)
        {
            t = (Time.time - startTime) / animationTime;

            transform.position = Vector3.Lerp(startPosition, _target.position, t);

            yield return new WaitForEndOfFrame();
        }
    }
}
