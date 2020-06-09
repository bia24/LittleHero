using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
public class TestAnimBind : MonoBehaviour
{
    private GameObject go;
    private static int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        go = Resources.Load<GameObject>("Player3");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject clone= GameObject.Instantiate(go);
            clone.name = clone.name + "_" + count;
            count++;
            AnimManager.instance.BindAnimCallBack<Player3>(clone.GetComponent<Animator>());
        }
    }
}
