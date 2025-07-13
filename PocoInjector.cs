using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Utils;
using Poco;
using UnityEngine;
using System.Collections.Generic;
using Il2CppInterop.Runtime;
using System;
using BepInEx.Configuration;

[BepInPlugin("com.example.pocoinjector", "Poco Injector", "1.0.0")]
public class PocoInjector : BasePlugin
{
    public override void Load()
    {
        Log.LogInfo("Poco Injector Loaded");
        MainThreadDispatcher.Initialize();
        
        myHotkey = Config.Bind("Hotkeys", "ToggleSomething", KeyCode.Q, "按键触发功能");
        Console.WriteLine("MyPlugin Loaded! Q");

        AddComponent<MainThreadDispatcher.DispatcherRunner>();
        AddComponent<PocoManager>();
        AddComponent<AutoName>();
        //AddComponent<TestManager>();
    }
    
    public ConfigEntry<KeyCode> myHotkey;

}

public static class MainThreadDispatcher
{
    private static readonly Queue<Action> _queue = new Queue<Action>();
    private static bool _initialized = false;

    /// <summary>
    /// 初始化调度器（必须在 Unity 主线程调用）
    /// </summary>
    public static void Initialize()
    {
        if (_initialized) return;

        //var go = new GameObject("MainThreadDispatcher");
        //UnityEngine.Object.DontDestroyOnLoad(go);
        //go.hideFlags = HideFlags.HideAndDontSave;
        //go.AddComponent<DispatcherRunner>();

        _initialized = true;
        Debug.Log("[MainThreadDispatcher] Initialized");
    }

    /// <summary>
    /// 在主线程执行任务
    /// </summary>
    public static void Run(Action action)
    {
        if (action == null) return;
        lock (_queue)
            _queue.Enqueue(action);
    }

    /// <summary>
    /// 每帧执行任务队列
    /// </summary>
    public class DispatcherRunner : MonoBehaviour
    {
        void Update()
        {
            lock (_queue)
            {
                while (_queue.Count > 0)
                {
                    var action = _queue.Dequeue();
                    try { action.Invoke(); }
                    catch (Exception ex) { Debug.LogError($"[MainThreadDispatcher] {ex}"); }
                }
            }
        }
    }
}


public class TestManager : MonoBehaviour
{

    UnityDumper dumper = new UnityDumper();

    public TestManager()
    {
        MonoBehaviourExtensions.StartCoroutine(this, loop());
    }


    public System.Collections.IEnumerator loop()
    {
        //Debug.Log("r");

        while (true)
        {
            var root = dumper.getRoot();
            foreach (var x in root.getChildren())
            {
                Debug.Log(x.ToString());
            }

            
           ;


            yield return null;
        }
    }

}



