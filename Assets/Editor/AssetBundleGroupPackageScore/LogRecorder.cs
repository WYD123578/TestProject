using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace AssetBundleGroupPackageScore
{
    public static class LogRecorder
    {
#if UNITY_EDITOR_WIN
        private static string SavePath = "E:/TempLog";
#elif UNITY_EDITOR_LINUX
        private static string SavePath = "/drone/src/temp";
#endif

        private static readonly Dictionary<string, StringBuilder>
            LogRecorders = new Dictionary<string, StringBuilder>();

        public static void Record(string recordKey, string logMessage)
        {
            if (!LogRecorders.ContainsKey(recordKey))
                LogRecorders.Add(recordKey, new StringBuilder());

            var logRecorder = LogRecorders[recordKey];
            logRecorder.Append(logMessage);
            logRecorder.Append("\t\n");
        }

        public static void Save(string recordKey, string savePath = null)
        {
            if (string.IsNullOrEmpty(savePath)) savePath = SavePath;

            if (!LogRecorders.ContainsKey(recordKey)) return;

            var logMsg = LogRecorders[recordKey].ToString();
            var logMsgBytes = Encoding.UTF8.GetBytes(logMsg);
            var fileName = recordKey + ".txt";
            try
            {
                File.WriteAllBytes(savePath + fileName, logMsgBytes);
            }
            catch (NullReferenceException e)
            {
                File.WriteAllBytes(Application.dataPath + fileName, logMsgBytes);
            }

            LogRecorders[recordKey].Clear();
            LogRecorders.Remove(recordKey);
        }
    }
}