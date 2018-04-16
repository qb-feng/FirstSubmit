using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TaskModel
{
    public MonoBehaviour mb;
    public Func<object[], IEnumerator> work;
    public object[] args;
}

//public enum TaskType
//{
//    None,
//    All,
//}

public class TaskManager : MonoBehaviour
{
    private static List<TaskModel> m_taskList = new List<TaskModel>();

    public static TaskManager Instance { get; set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(YieldTask());
    }

    /// <summary>  
    /// 线程工作的函数  
    /// </summary>  
    private IEnumerator YieldTask()
    {
        while (true)
        {
            //获取任务  
            var task = Pop();
            //执行任务 
            if (task != null && task.mb && task.mb.isActiveAndEnabled)
            {
                yield return task.work(task.args);
            }
            else
                yield return 0;
        }
    }

    /// <summary>  
    /// 从任务队列中取出任务  
    /// </summary>  
    /// <returns></returns>  
    public TaskModel Pop()
    {
        TaskModel ac = null;
        //当队列有数据，出队.否则等待  
        if (m_taskList.Count > 0)
        {
            ac = m_taskList[0];
            m_taskList.Remove(ac);
        }
        return ac;
    }

    public void Remove(TaskModel task)
    {
        if (task != null)
        {
            if (m_taskList.Contains(task))
                m_taskList.Remove(task);
        }
    }

    /// <summary>  
    /// 把任务加入任务队列  
    /// </summary>  
    public void Push(TaskModel task)
    {
        //把任务加入队列中  
        m_taskList.Add(task);
    }
}
