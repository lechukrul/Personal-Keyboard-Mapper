using System;
using System.Linq;
using System.Text;
using Personal_Keyboard_Mapper.Lib.Enums;
using Personal_Keyboard_Mapper.Lib.Extensions;
using Personal_Keyboard_Mapper.Lib.Interfaces;
using Action = Personal_Keyboard_Mapper.Lib.Model.Action;

namespace Personal_Keyboard_Mapper.Lib.Prediction
{
    public class WordPredictionService
    {
        private StringBuilder _currentWord;
        private WordFrequencyModel _model;
        private string _acceptanceFirstKey;
        private string _acceptanceSecondKey;

        public event System.Action<string> PredictionChanged;

        public WordPredictionService(string dataFilePath)
        {
            _currentWord = new StringBuilder();
            _model = new WordFrequencyModel(dataFilePath);
        }

        public void SetAcceptanceCombination(string firstKey, string secondKey)
        {
            _acceptanceFirstKey = firstKey;
            _acceptanceSecondKey = secondKey;
        }

        public bool IsAcceptanceCombination(IKeyCombination combination)
        {
            if (_acceptanceFirstKey == null || _acceptanceSecondKey == null) return false;
            return combination.HasKeys(_acceptanceFirstKey, _acceptanceSecondKey);
        }

        private static readonly WindowsInput.Native.VirtualKeyCode[] _wordBoundaryKeys =
        {
            (WindowsInput.Native.VirtualKeyCode)0xBC,  // ,
            (WindowsInput.Native.VirtualKeyCode)0xBE,  // .
            (WindowsInput.Native.VirtualKeyCode)0xBA,  // ; :
        };

        private static readonly System.Collections.Generic.Dictionary<int, char> _polishLowerMap =
            new System.Collections.Generic.Dictionary<int, char>
            {
                { 0x41, 'ą' },
                { 0x43, 'ć' },
                { 0x45, 'ę' },
                { 0x4C, 'ł' },
                { 0x4F, 'ó' },
                { 0x53, 'ś' },
                { 0x58, 'ź' },
                { 0x5A, 'ż' },
            };

        public void ProcessAction(Action action)
        {
            if (action == null) return;
            if (action.Type == ActionType.Mouse) return;
            if (action.IsModKeyAction()) return;

            if (action.VirtualKeys == null || !action.VirtualKeys.Any()) return;


            bool shiftActive = Globals.IsShiftPressedOnce || Globals.IsShiftHoldDown;

            bool isExclamationMark = shiftActive && action.VirtualKeys.Any(v => v == WindowsInput.Native.VirtualKeyCode.VK_1);

            bool isBoundaryStringKey = action.ActionStringKeys != null
                && (action.ActionStringKeys.Any(k => k == "?") || isExclamationMark);

            if (isBoundaryStringKey
                || action.VirtualKeys.Any(v => v == WindowsInput.Native.VirtualKeyCode.SPACE
                                            || v == WindowsInput.Native.VirtualKeyCode.RETURN
                                            || _wordBoundaryKeys.Contains(v)))
            {
                FinalizeCurrentWord();
                return;
            }

            if (action.VirtualKeys.Any(v => v == WindowsInput.Native.VirtualKeyCode.BACK))
            {
                if (_currentWord.Length > 0)
                    _currentWord.Remove(_currentWord.Length - 1, 1);
                NotifyPredictionChanged();
                return;
            }

            if (action.VirtualKeys.Count == 1)
            {
                var vkInt = (int)action.VirtualKeys[0];
                if (vkInt >= 0x41 && vkInt <= 0x5A)
                {
                    bool altActive = Globals.IsRightAltPressedOnce || Globals.IsRightAltHoldDown
                                  || Globals.IsLeftAltPressedOnce || Globals.IsLeftAltHoldDown;
                    if (altActive && _polishLowerMap.TryGetValue(vkInt, out var polishLower))
                    {
                        bool upper = Globals.IsShiftPressedOnce || Globals.IsShiftHoldDown;
                        _currentWord.Append(upper ? char.ToUpperInvariant(polishLower) : polishLower);
                        NotifyPredictionChanged();
                        return;
                    }

                    var ch = (char)('a' + vkInt - 0x41);
                    if (Globals.IsShiftPressedOnce || Globals.IsShiftHoldDown)
                        ch = char.ToUpperInvariant(ch);
                    _currentWord.Append(ch);
                    NotifyPredictionChanged();
                    return;
                }
            }
            ClearBuffer();
        }

        public string AcceptPrediction()
        {
            var prefix = _currentWord.ToString().ToLowerInvariant();
            if (string.IsNullOrEmpty(prefix)) return null;
            var predicted = _model.Predict(prefix);
            if (predicted == null || predicted.Length <= prefix.Length) return null;
            var suffix = predicted.Substring(prefix.Length);
            LearnAndReset(predicted);
            return suffix;
        }

        public void ClearBuffer()
        {
            _currentWord.Clear();
            PredictionChanged?.Invoke(null);
        }

        private void FinalizeCurrentWord()
        {
            var word = _currentWord.ToString().ToLowerInvariant();
            LearnAndReset(word);
        }

        private void LearnAndReset(string word)
        {
            if (!string.IsNullOrEmpty(word) && word.Length >= 2)
                _model.LearnWord(word);
            _currentWord.Clear();
            PredictionChanged?.Invoke(null);
        }

        private void NotifyPredictionChanged()
        {
            var prefix = _currentWord.ToString().ToLowerInvariant();
            var predicted = prefix.Length >= 2 ? _model.Predict(prefix) : null;
            PredictionChanged?.Invoke(predicted);
        }
    }
}
