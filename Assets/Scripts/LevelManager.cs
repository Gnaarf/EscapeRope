using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    Transform Tutorial;

    [SerializeField]
    Transform FirstLevel;

    [SerializeField]
    float _animationTime = 3F;

    [SerializeField]
    TutorialMoverWall[] tutorialMoverWalls;

    bool _hasStarted = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(!_hasStarted && Input.GetButton("StartGame"))
        {
            _hasStarted = true;
            StartCoroutine(SwitchFromTutorialToFirstLevel());
        }
    }

    IEnumerator SwitchFromTutorialToFirstLevel()
    {
        foreach(TutorialMoverWall wall in tutorialMoverWalls)
        {
            wall.StartMoving(_animationTime);
        }

        yield return new WaitForSeconds(_animationTime);

        Tutorial.gameObject.SetActive(false);
        FirstLevel.gameObject.SetActive(true);

    }
}
