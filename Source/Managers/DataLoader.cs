using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

namespace ScrapBox.Managers
{
    public class DataLoader
    {
        private readonly Dictionary<char, int> endCharacters;
        private Dictionary<string, string> values;

        public DataLoader()
        {
            endCharacters = new Dictionary<char, int>() { [']'] = 1, ['"'] = 2 };
            values = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the index of the nth occurance of a character in a string
        /// </summary>
        /// <param name="s">Source string</param>
        /// <param name="c">Search character</param>
        /// <param name="n">Nth occurance</param>
        /// <returns>The index of the nth occurance in the source string</returns>
        private int GetNthIndex(string s, char c, int n)
        {
            short count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == c) count++;
                if (count == n) return i;
            }

            return -1;
        }

        /// <summary>
        /// Check if a field exists in the sData file
        /// </summary>
        /// <param name="key">The field from the sData file</param>
        /// <returns>True if it exists, otherwise false</returns>
        public bool KeyExists(string key)
        {
            return values.ContainsKey(key);
        }

        /// <summary>
        /// Gets data from the sData file and converts it to the appropriate type
        /// </summary>
        /// <typeparam name="T">Base types + int[] are supported</typeparam>
        /// <param name="key">The name of the field in the json file</param>
        /// <returns></returns>
        public T Fetch<T>(string key)
        {
            if (!KeyExists(key)) throw new Exception("Key doesn't exist");

            //Special cases where conventional convert doesn't work
            if (typeof(T) == typeof(int[]))
            {
                return (T)(object)values[key].Split(',').Select(int.Parse).ToArray();
            }
            else if (typeof(T) == typeof(Vector2))
            {
                int[] tempArray = values[key].Split(',').Select(int.Parse).ToArray();
                return (T)(object)new Vector2(tempArray[0], tempArray[1]);
            }
            else if (typeof(T) == typeof((int, int)))
            {
                int[] tempArray = values[key].Split(',').Select(int.Parse).ToArray();
                return (T)(object)(tempArray[0], tempArray[1]);
            }
            else if (typeof(T) == typeof(int[,]))
            {
                string oneDim = String.Join(string.Empty, values[key].Split(',')).Trim();
                oneDim = oneDim.Replace("\n", string.Empty);
                oneDim = oneDim.Replace("\t", string.Empty);

                //Pre scan for column size
                int columnSize = 0;
                for (int i = 0; i < oneDim.Length; i++)
                {
                    char c = oneDim[i];
                    if (c == '/')
                    {
                        columnSize = i;
                        break;
                    }
                }
                
                int rows = oneDim.Length / columnSize;
                int[,] twoDim = new int[columnSize, rows];
                
                int currentColumn = 0;
                int currentRow = 0;
                for (int i = 0; i < oneDim.Length; i++)
                {
                    char c = oneDim[i];

                    
                    if (c == '/')
                    {
                        currentColumn = 0;
                        currentRow++;
                        continue;
                    }
                        
                    twoDim[currentColumn, currentRow] = int.Parse(c.ToString());
                    currentColumn++;
                }

                return (T)(object)twoDim;
            }

            return (T)Convert.ChangeType(values[key], typeof(T));
        }

        /// <summary>
        /// Load a raw sData file and process it by extracting field names and values
        /// </summary>
        /// <param name="data">The data file in raw text format</param>
        public void ReadData(string data)
        {
            int skipIndex = 0;
            for (int i = 0; i < data.Length; i++)
            {
                
                char curChar = data[i];
                if (char.IsWhiteSpace(curChar)) continue;

                if (curChar == ':')
                {
                    int startIndex = GetNthIndex(data.Substring(skipIndex, Math.Abs(i-skipIndex)), '"', 1);
                    startIndex += skipIndex;

                    int endIndex = int.MaxValue;
                    foreach (KeyValuePair<char, int> p in endCharacters)
                    {
                        int nthIndex = GetNthIndex(data[i..], p.Key, p.Value);
                        if (nthIndex == -1) continue;

                        if (nthIndex + i + skipIndex < endIndex)
                        {
                            endIndex = nthIndex + i;
                        }
                    }

                    string line = data.Substring(startIndex, Math.Abs(endIndex-skipIndex)-2);

                    skipIndex += endIndex-skipIndex+2;

                    line = line.Replace(" ", string.Empty);
                    line = line.Replace("\"", string.Empty);
                    line = line.TrimEnd();

                    if (line[line.Length-1] == ',')
                    {
                        line = line.Substring(0, line.Length - 1);
                    }

                    line = line.Replace("[", string.Empty);
                    line = line.Replace("]", string.Empty);

                    int separator = GetNthIndex(line, ':', 1);
                    values.Add(line.Substring(0, separator), line.Substring(separator + 1));
                    
                }
            }
        }
    }
}
