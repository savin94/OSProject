using System;
using System.Collections.Generic;

namespace HaikuStudent
{
    public class HaikuBuilder
    {
        private List<string> _currentHaiku = new List<string>();
        private int _wordCount = 0;
        private readonly List<int> _structure = new List<int> { 5, 7, 5 }; // Haiku structure
        private int _currentLine = 0;
        private readonly HaikuLogger _logger;

        public void AddWord(string word)
        {
            _currentHaiku.Add(word);
            _wordCount++;

            int requiredWords = _structure[_currentLine];

            if (_wordCount == requiredWords)
            {
                PrintHaikuLine();

                // Move to the next line in the Haiku
                _currentLine++;
                _wordCount = 0;

                // If the Haiku is complete, reset and log it
                if (_currentLine == _structure.Count)
                {
                    ResetHaiku();
                }
            }
        }

        private void PrintHaikuLine()
        {
            var line = string.Join(" ", _currentHaiku.ToArray(), _currentHaiku.Count - _wordCount, _wordCount);
            Console.WriteLine(line);
        }

        private void ResetHaiku()
        {
            _currentHaiku.Clear();
            _currentLine = 0;
            _wordCount = 0;
        }
    }
}
