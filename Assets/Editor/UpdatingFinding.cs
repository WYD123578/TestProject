using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class UpdatingFinding
{
    public class Node
    {
        public string name;
        public float time;
        
    }
    
    [MenuItem("Finding/Updating")]
    public static void FindUpdating()
    {
        /*var inputString = @"Updating ProjectSettings/GraphicsSettings.asset - GUID: 00000000000000006100000000000000...
 done. [Time: 2.127000 ms] 
Updating ProjectSettings/TimeManager.asset - GUID: 00000000000000007000000000000000...
 done. [Time: 1.475000 ms] 
Updating ProjectSettings/ClusterInputManager.asset - GUID: 00000000000000007100000000000000...
 done. [Time: 1.662000 ms] 
Updating ProjectSettings/DynamicsManager.asset - GUID: 00000000000000008000000000000000...
 done. [Time: 20.415000 ms] 
Updating ProjectSettings/QualitySettings.asset - GUID: 00000000000000009000000000000000...
 done. [Time: 2.370000 ms] 
Updating ProjectSettings/NetworkManager.asset - GUID: 0000000000000000a000000000000000...
 done. [Time: 5.206000 ms] ";*/
        var inputString = File.ReadAllText(Application.dataPath + "/log.log");
        //var pattern = @"[Uu]pdating ([a-zA-Z0-9/\.]*) - GUID: ([a-zA-Z0-9]+...)[\r\n] done. ([Time: [0-9]+(\\.[0-9]+)? ms)]";
        var pattern = @"[Uu]pdating (.+)? - GUID: ((.+)?...)[\r\n](.*)?(done.) \[Time: (.+)? ms\]";
        var results = Regex.Matches(inputString, pattern);
        Debug.Log($"找到{results.Count.ToString()}个匹配项");
        var listNode = new List<Node>();
        foreach (Match result in results)
        {
            var str = result.Value;
            var subStrs = Regex.Split(str,@"- GUID: ");
            var name =  subStrs[0].Split(' ')[1];
            var timeStr = Regex.Match(subStrs[1], @"[0-9]+(\.[0-9]+)? ms").Value.Split(' ')[0];
            var time = 0f;
            if(timeStr!="") time = float.Parse(timeStr);
            var node = new Node(){name = name, time = time};
            listNode.Add(node);
        }
        listNode.Sort((node1, node2) => node1.time < node2.time ? -1 : 1);

        using (var fs = new FileStream(Application.dataPath+"/res.csv", FileMode.OpenOrCreate))
        {
            var sb = new StringBuilder();
            sb.Append("资源名字,加载耗时\r\n");
            foreach (var node in listNode)
            {
                sb.Append(node.name);
                sb.Append(',');
                sb.Append(node.time);
                sb.Append("\r\n");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            fs.Write(bytes, 0, bytes.Length);
        }
    }
}
