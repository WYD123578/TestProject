using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace AssetBundleGroupPackageScore
{
    public static class LogRecorder
    {
        public static string SavePath = "E:/";
        private static Dictionary<string, StringBuilder> _logRecorders = new Dictionary<string, StringBuilder>();

        public static void Record(string recordKey, string logMessage)
        {
            if (!_logRecorders.ContainsKey(recordKey))
                _logRecorders.Add(recordKey, new StringBuilder());

            var logRecorder = _logRecorders[recordKey];
            logRecorder.Append(logMessage);
            logRecorder.Append("\t\n");
        }

        public static void Save(string recordKey)
        {
            if (!_logRecorders.ContainsKey(recordKey)) return;

            var logMsg = _logRecorders[recordKey].ToString();
            var logMsgBytes = Encoding.UTF8.GetBytes(logMsg);
            var fileName = recordKey + ".txt";
            try
            {
                File.WriteAllBytes(SavePath + fileName, logMsgBytes);
            }
            catch (NullReferenceException e)
            {
                File.WriteAllBytes(Application.dataPath + fileName, logMsgBytes);
            }
        }
    }
}