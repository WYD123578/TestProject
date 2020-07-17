using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace AssetBundleGroupPackageScore.Score
{
    public class GetScoreFromSuffix : IScoreGetMethod
    {
        private readonly Dictionary<string, int> _scoreMap;
        private readonly Dictionary<string, string> _suffixMap;

        public GetScoreFromSuffix(string scoreMapJsonPath, string suffixMapJsonPath)
        {
            var scoreMapJson = Encoding.UTF8.GetString(File.ReadAllBytes(scoreMapJsonPath));
            var suffixMapJson = Encoding.UTF8.GetString(File.ReadAllBytes(suffixMapJsonPath));
            // -----------解析scoreMap-----------
            var scoreMapObj = Json.Deserialize(scoreMapJson) as Dictionary<string, object>;
            _scoreMap = new Dictionary<string, int>();
            if (scoreMapObj != null)
                foreach (var kvp in scoreMapObj)
                {
                    var key = kvp.Key;
                    var value = Convert.ToInt32(kvp.Value);
                    _scoreMap.Add(key, value);
                }

            // -----------解析suffixMap-----------
            var suffixMapObj = Json.Deserialize(suffixMapJson) as Dictionary<string, object>;
            _suffixMap = new Dictionary<string, string>();
            if (suffixMapObj != null)
                foreach (var kvp in suffixMapObj)
                {
                    var key = kvp.Key;
                    var value = kvp.Value.ToString();
                    _suffixMap.Add(key, value);
                }
            
            Debug.Log("================获取评分细则完成================");
        }

        public int GetScoreFromMap(string assetName)
        {
            var suffix = assetName.Split('\\', '/').Reverse().ToArray()[0].Split('.')[1];
            string assetType;
            if (_suffixMap.TryGetValue(suffix, out assetType))
            {
                int score;
                if (_scoreMap.TryGetValue(assetType, out score))
                {
                    return score;
                }
            }

            return 1;
        }
    }
}