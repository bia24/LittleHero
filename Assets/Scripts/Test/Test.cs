using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

public class Test : MonoBehaviour
{
   public GameObject go = null;
    // Start is called before the first frame update
    void Start()
    {
        go.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            show();
        }
    }

    private void show()
    {
        go.SetActive(true);
        Debug.Log("show");
        Debug.Log("show");
        Debug.Log("show");
        Debug.Log("show");
        Debug.Log("show");
        Debug.Log("show");
    }
 
}
