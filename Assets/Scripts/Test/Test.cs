using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using DG.Tweening;
public class Test : MonoBehaviour
{
    public AnimationClip anim;
    // Start is called before the first frame update
    private void Start()
    {
        

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Down"+Time.frameCount);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            Debug.Log("up" + Time.frameCount);
        }
    }
}
