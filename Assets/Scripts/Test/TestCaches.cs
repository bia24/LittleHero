using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using UnityEngine.SceneManagement;

public class TestCaches : MonoBehaviour
{
    public GameObject go = null;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        go = Resources.Load<GameObject>("Test/Cube");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            go=Instantiate(go);
        }
        if (Input.GetMouseButtonDown(1))
        {
            SceneManager.LoadSceneAsync(1);
           
        }
    }
}
