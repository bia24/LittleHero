using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGModule {
    public class SceneManager : Singleton<SceneManager>
    {
        private GameObject sceneLoader;

        public SceneManager()
        {
            sceneLoader = GameObject.Find("SceneLoader");
            if (sceneLoader == null)
                sceneLoader = new GameObject("SceneLoader");
            Object.DontDestroyOnLoad(sceneLoader);
        }

        public void StartLoadScene(string sceneName)
        {
            SceneLoader loader = sceneLoader.AddComponent<SceneLoader>();
            loader.StartLoadScene(sceneName);
        }
    }
}
