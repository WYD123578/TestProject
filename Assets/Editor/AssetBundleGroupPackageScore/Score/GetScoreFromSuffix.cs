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
            _scoreMap = Deserialize.To<Dictionary<string, int>>(scoreMapJson);
            _suffixMap = Deserialize.To<Dictionary<string, string>>(suffixMapJson);
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