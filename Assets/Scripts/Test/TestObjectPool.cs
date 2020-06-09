using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

public class TestObjectPool : MonoBehaviour
{
    GameObject f;
    private void Start()
    {
        f=PoolManager.instance.getPrefab("father");
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PoolManager.instance.getPrefab("player",f.transform);
        }
        if (Input.GetMouseButtonDown(1))
        {
            for(int i = 0; i < f.transform.childCount; i++)
            {
                if (f.transform.GetChild(i).gameObject.activeSelf)
                {
                    PoolManager.instance.RevertPool("player", f.transform.GetChild(i).gameObject);
                    break;
                }
            }
        }
        
    }


   
}
