using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;

public class TestLoadConfig : MonoBehaviour
{
   
    // Start is called before the first frame update
    void Start()
    {
       

    }

    private void Callback(string res)
    {
        Debug.Log("i have get res: "+res);
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DataManager.instance.SaveData("input","hahah new data");
        }

        if (Input.GetMouseButtonDown(1))
        {
            DataManager.instance.LoadData("input", Callback);
        }
    }
}
