using BepInEx.Unity.IL2CPP.Utils;
using Poco;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading;
using BepInEx.Unity.IL2CPP;

public class AutoName : MonoBehaviour
{
    UnityDumper dumper = new UnityDumper();
    private Thread thread;
    List<string> namesList;
    private int idx = 0;
    private string lastName = "";

    public AutoName()
    {
        string nameChars = "伟强明华丽娜婷杰勇军磊涛斌鑫浩超鹏辉波刚平娟颖芳静霞燕敏洋帅晨楠宇轩涵泽欣怡梦雪睿梓豪"
                                              + "坤坤东西南北春夏秋冬天雨雷风云龙虎豹鹰骏驰飞翔翰才思文艺书琴棋画音诗晨曦星月辰光耀晨宇晨曦浩然"
                                              + "嘉瑞成诚善良淼亮畅宏志智慧安康宁静若兰子怡子涵梓萱梓童梓琪梓悦梓桐梓豪梓瑞梓妍梓涵梓杰梓宇"
                                              + "一二三四五六七八九十百千万亿凡尧航航宇泽林泽宇泽楷泽浩泽轩泽阳泽辰泽东泽西泽南泽北";


        System.Random random = new System.Random();
        HashSet<string> names = new HashSet<string>();

        while (names.Count < 10000)
        {
            // int len = random.Next(1, 3); // 名字长度 1 或 2
            char[] chars = new char[2];
            for (int i = 0; i < 2; i++)
            {
                chars[i] = nameChars[random.Next(nameChars.Length)];
            }
            names.Add(new string(chars));
        }
        namesList = new List<string>(names);
        
        // thread = new Thread(t);
        // //thread.IsBackground = true;
        // thread.Start();
    }

    public void t()
    {
        while (true)
        {
            Thread.Sleep(2000);
            // Console.WriteLine("run??");
            MainThreadDispatcher.Run(() => {
                timeout();
            });
        }
    }

    public void timeout()
    {
        // if (!dumper.ContainNode("取个名字吧")) return;

        var node = dumper.GetFieldNameNode();
        if (node == null) return;

        var unode = (UnityNode)node;
        string curName = UnityNode.GetText(unode.gameObject);
        if (curName != lastName)
        {
            string newName = namesList[idx++];
            UnityNode.SetText(unode.gameObject, newName);
            lastName = newName;
        }
        
    }
    
    public void Update()
    {
        // 注意：这里要访问外部类的静态实例才能拿到配置
        if (Input.GetKeyDown(((PocoInjector)IL2CPPChainloader.Instance.Plugins["com.example.pocoinjector"].Instance).myHotkey.Value))
        {
            // ((PocoInjector)IL2CPPChainloader.Instance.Plugins["com.example.pocoinjector"].Instance).OnHotkeyPressed();
            OnHotkeyPressed();
        }
    }

    public void OnHotkeyPressed()
    {
        // Console.WriteLine("OnHotkeyPressed??");
        timeout();
    }
}
