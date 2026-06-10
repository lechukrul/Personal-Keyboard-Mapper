using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Personal_Keyboard_Mapper.Lib.Prediction
{
    public class WordFrequencyModel
    {
        private Dictionary<string, int> _frequencies;
        private readonly string _dataFilePath;

        public WordFrequencyModel(string dataFilePath)
        {
            _dataFilePath = dataFilePath;
            _frequencies = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            Load();
        }

        public void LearnWord(string word)
        {
            if (string.IsNullOrEmpty(word) || word.Length < 2) return;
            word = word.ToLowerInvariant();
            _frequencies[word] = _frequencies.TryGetValue(word, out var count) ? count + 1 : 1;
            Save();
        }

        public string Predict(string prefix)
        {
            if (string.IsNullOrEmpty(prefix) || prefix.Length < 2) return null;
            prefix = prefix.ToLowerInvariant();
            return _frequencies
                .Where(kv => kv.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                             && kv.Key.Length > prefix.Length)
                .OrderByDescending(kv => kv.Value)
                .Select(kv => kv.Key)
                .FirstOrDefault();
        }

        private void Save()
        {
            try
            {
                File.WriteAllText(_dataFilePath, JsonConvert.SerializeObject(_frequencies, Formatting.None));
            }
            catch { }
        }

        private void Load()
        {
            try
            {
                if (!File.Exists(_dataFilePath)) return;
                var json = File.ReadAllText(_dataFilePath);
                _frequencies = JsonConvert.DeserializeObject<Dictionary<string, int>>(json)
                    ?? new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                PurgeRareWords();
            }
            catch { }
        }

        private void PurgeRareWords()
        {
            var rare = _frequencies.Where(kv => kv.Value <= 1).Select(kv => kv.Key).ToList();
            if (rare.Count == 0) return;
            foreach (var word in rare)
                _frequencies.Remove(word);
            Save();
        }
    }
}
