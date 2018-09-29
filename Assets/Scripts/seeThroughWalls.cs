using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class seeThroughWalls : MonoBehaviour {

    public bool playerOne;
    Camera mainCam;
	// Use this for initialization
	void Start () {
        mainCam = FindObjectOfType<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 ScreenPos = mainCam.WorldToScreenPoint(this.gameObject.transform.position);
        ScreenPos = new Vector2(ScreenPos.x/ mainCam.pixelWidth , ScreenPos.y / mainCam.pixelHeight);
        if (playerOne)
        {

            Shader.SetGlobalVector("_PlayerScreenPos", ScreenPos);
            Shader.SetGlobalVector("_PlayerPos", this.transform.position);
        } else
        {
            Shader.SetGlobalVector("_PlayerScreenPos2", ScreenPos);
            Shader.SetGlobalVector("_PlayerPos2", this.transform.position);
        }
        Shader.SetGlobalVector("_CamDir", mainCam.transform.forward);
    }
}
