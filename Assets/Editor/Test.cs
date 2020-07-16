using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Test
{

    [MenuItem("Tool/Build")]
    public static void BuildTestMenuItem()
    {
        string path = Application.dataPath + "/../qqq.apk";
    }

    public static void BuildTest()
    {
        Debug.Log("========================================================");
        Debug.Log("BuildTest");
        Debug.Log("========================================================");

        //string path = Application.dataPath + "/../qqq.apk";
        Debug.Log("========================================================");
        Debug.Log("======================================================== 1. ");
        Debug.Log("======================================================== 2. ");
    }
}
