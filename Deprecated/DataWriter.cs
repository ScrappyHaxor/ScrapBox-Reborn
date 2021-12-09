using System;
using System.IO;
using System.Text;

using Microsoft.Xna.Framework;

namespace ScrapBox.Framework.Deprecated
{
    [Obsolete("Don't use this mess")]
    public class DataWriter
    {
        private StringBuilder buffer;
        string destination;

        public DataWriter()
        {
            buffer = new StringBuilder();
        }

        public void BeginWrite(string path)
        {
            destination = path;
            buffer.AppendLine("{");
        }

        public void Serialize<T>(string fieldName, T fieldValue, int columnBreak = 0)
        {
            buffer.Append($"\t\"{fieldName}\": ");

            if (typeof(T) == typeof(int[]))
            {
                int[] data = (int[])(object)fieldValue;
                buffer.AppendLine("[");

                if (columnBreak != 0)
                {
                    buffer.Append("\t\t");
                }

                int columnBreakCount = 0;
                for (int i = 0; i < data.Length; i++)
                {
                    if (columnBreak != 0)
                    {
                        if (columnBreakCount == columnBreak)
                        {
                            buffer.Append("\n\t\t");
                            columnBreakCount = 0;
                        }

                        if (i == data.Length - 1)
                        {
                            buffer.Append($"{data[i]}");
                        }
                        else
                        {
                            buffer.Append($"{data[i]},");
                        }
                        columnBreakCount++;
                    }
                    else
                    {
                        if (i == data.Length - 1)
                        {
                            buffer.AppendLine($"\t\t{data[i]}");
                        }
                        else
                        {
                            buffer.AppendLine($"\t\t{data[i]},");
                        }
                    }
                }

                if (columnBreak != 0)
                {
                    buffer.Append("\n");
                }

                buffer.AppendLine("\t],");
                return;
            }
            else if (typeof(T) == typeof(Vector2))
            {
                throw new NotImplementedException();
            }
            else if (typeof(T) == typeof((int, int)))
            {
                throw new NotImplementedException();
            }

            buffer.Append($"\"{fieldValue}\",");
        }

        public void EndWrite()
        {
            string tempString = buffer.ToString();
            int trailingComma = tempString.LastIndexOf(",");
            if (trailingComma != -1)
            {
                tempString = tempString.Substring(0, trailingComma);
            }

            buffer = new StringBuilder(tempString);
            buffer.Append("\n}");

            File.WriteAllText(destination, buffer.ToString());
        }
    }
}
