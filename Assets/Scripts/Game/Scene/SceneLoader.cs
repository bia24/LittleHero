using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SGModule;


public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// 异步加载操作数
    /// </summary>
    AsyncOperation task;
    /// <summary>
    /// 任务开启/关闭标志
    /// </summary>
    bool isStart = false;
    /// <summary>
    /// 本次加载任务的名称
    /// </summary>
    string taskName;


    void Update()
    {
        if (isStart)
        {
            if (!task.isDone)
            {
                //当任务开始时，要将操作数进度发送出去
                EventCenter.Instance.SendEvent(SGEventType.SceneLoadProcess, new EventData(task.progress, null));
            }
            else
            {
                //任务结束，进度条到0.9，防止下次update再进行判断
                isStart = false;
                //开启最后0.1的协程部分
                StartCoroutine(TaskDone());
            }
        }
    }
    /// <summary>
    /// 场景加载任务开启
    /// </summary>
    /// <param name="sceneName"></param>
    public void StartLoadScene(string sceneName)
    {
        task = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        taskName = sceneName;
        isStart = true;
    }
    /// <summary>
    /// 任务0.9后最后0.1的协程显示部分
    /// </summary>
    /// <returns></returns>
    IEnumerator TaskDone()
    {
        float p = 0.9f;
        while (p < 1.0f)
        {
            EventCenter.Instance.SendEvent(SGEventType.SceneLoadProcess, new EventData(p, null));
            p += Time.deltaTime;
            yield return null;
        }
        EventCenter.Instance.SendEvent(SGEventType.SceneLoadProcess, new EventData(1.0f, null));
        yield return new WaitForSeconds(1.0f);
        //完成本次场景加载任务
        Finished();
    }
    /// <summary>
    /// 完成本次任务的后续操作
    /// </summary>
    private void Finished()
    {
        EventCenter.Instance.SendEvent(SGEventType.SceneLoadDone, new EventData(taskName, null));
        Destroy(this);
    }
}

