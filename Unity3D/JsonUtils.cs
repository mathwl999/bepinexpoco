
using System;
using System.Collections.Generic;
using System.Text;
using TinyJson;


public static class JsonUtil
{
    /// <summary>
    /// 安全解析 JSON 字符串，失败返回 null
    /// </summary>
    public static Dictionary<string, object> ParseObject(string json)
    {
        return json.FromJson<Dictionary<string, object>>();

    }

  
    public static string ToJson(object obj)
    {
        var ret = obj.ToJson();
        //Console.WriteLine(ret);
        return ret;
    }

    


}
