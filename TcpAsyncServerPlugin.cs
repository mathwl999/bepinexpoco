using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Unity.IL2CPP;
using Il2CppInterop.Runtime;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TriangleNet;
using UnityEngine;

//[BepInPlugin("com.example.tcpasync", "TCP Async Server", "1.0")]
//public class TcpAsyncServerPlugin : BasePlugin
//{
//    private TcpListener listener;
//    private bool running;

//    // ✅ 简易主线程任务队列
//    private static readonly Queue<Action> mainThreadQueue = new Queue<Action>();

//    public override void Load()
//    {
//        running = true;
//        StartServerLoop();

//        // 注册 Update 循环
//        AddComponent<MainThreadRunner>();

//        Log.LogInfo("Async TCP Server started on 127.0.0.1:5001");
//    }

//    private async void StartServerLoop()
//    {
//        listener = new TcpListener(IPAddress.Loopback, 5001);
//        listener.Start();

//        while (running)
//        {
//            try
//            {
//                TcpClient client = await listener.AcceptTcpClientAsync();
//                EnqueueOnMainThread(() =>
//                {
//                    Debug.Log("client ?" + client);
//                });
                    
//                _ = HandleClientAsync(client);
//            }
//            catch (ObjectDisposedException)
//            {
//                break;
//            }
//            catch (Exception ex)
//            {
//                Log.LogError("AcceptTcpClientAsync error: " + ex);
//            }
//        }
//    }

//    private async Task HandleClientAsync(TcpClient client)
//    {
//        using (client)
//        using (var stream = client.GetStream())
//        {
//            byte[] buffer = new byte[1024];

//            while (true)
//            {
//                int bytes;
//                try
//                {
//                    bytes = await stream.ReadAsync(buffer, 0, buffer.Length);
//                    if (bytes == 0) break;
//                }
//                catch { break; }

//                string msg = Encoding.UTF8.GetString(buffer, 0, bytes);
//                Log.LogInfo("Received: " + msg);

//                if (msg == "LIST")
//                {
//                    // ✅ 把任务丢到主线程执行
//                    EnqueueOnMainThread(() =>
//                    {
//                        Debug.Log("run ?");
//                        var objects = UnityEngine.Object.FindObjectsOfType(Il2CppType.From(typeof(GameObject)));
//                        StringBuilder sb = new StringBuilder();

//                        foreach (var o in objects)
//                        {
//                            var go = o.TryCast<GameObject>();
//                            if (go != null)
//                                sb.AppendLine(go.name);
//                        }

//                        byte[] data = Encoding.UTF8.GetBytes(sb.ToString());
//                        try { stream.Write(data, 0, data.Length); } catch { }
//                    });
//                }
//            }
//        }
//    }

//    public override bool Unload()
//    {
//        running = false;
//        listener?.Stop();
//        return true;
//    }

//    // --- 主线程调度工具 ---
//    public static void EnqueueOnMainThread(Action action)
//    {
//        lock (mainThreadQueue)
//            mainThreadQueue.Enqueue(action);
//    }

//    private class MainThreadRunner : MonoBehaviour
//    {
//        public void Update()
//        {
//            lock (mainThreadQueue)
//            {
//                while (mainThreadQueue.Count > 0)
//                {
//                    var action = mainThreadQueue.Dequeue();
//                    try { action.Invoke(); }
//                    catch (Exception ex) { Debug.LogError(ex.ToString()); }
//                }
//            }
//        }
//    }
//}
