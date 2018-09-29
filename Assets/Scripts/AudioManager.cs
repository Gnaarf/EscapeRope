using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    
    AudioSource ad;
    public AudioSource OneShot;
    public static AudioManager sg;
    public AudioClip bubbleSound;
	// Use this for initialization
	void Start () {
        if (sg == null) sg = this;
        else Destroy(this.gameObject);
        ad = this.GetComponent<AudioSource>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayOneShot()
    {
        float min = -0.3f;
        float max = 0.3f;

        float value = min + Random.value * (max - min);


        OneShot.pitch = ad.pitch + value;

        OneShot.PlayOneShot(bubbleSound);


    }
}
