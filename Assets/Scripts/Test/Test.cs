using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       TextAsset t=  AssetManager.instance.LoadObject<TextAsset>("hello");
        Debug.Log("i got it");
    }

    private void Update()
    {
        Debug.Log("lloop[[");
    }
}
