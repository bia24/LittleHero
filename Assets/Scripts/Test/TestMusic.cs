using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMusic : MonoBehaviour
{
    public AudioClip clip1;
    public AudioClip clip2;
    private AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source= gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            source.PlayOneShot(clip1,0.1f);
            Debug.Log(source.isPlaying);
        }
        if (Input.GetMouseButtonDown(1))
        {
            source.PlayOneShot(clip2,0.1f);
            Debug.Log(source.isPlaying);
        }
        //if (Time.frameCount % 480 == 0)
        //{
        //    Debug.Log("main : " +source.isPlaying);
        //}
    }
}
