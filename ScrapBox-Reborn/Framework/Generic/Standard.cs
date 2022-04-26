using System;
using System.Collections.Generic;
using System.Text;

using ScrapBox.Framework.Math;

namespace ScrapBox.Framework.Generic
{
    public static class Standard
    {
		public static void Swap<T>(ref T a, ref T b)
		{
			T tmp = a;
			a = b;
			b = tmp;
		}

		public static T FetchIndex<T>(int index, T[] array)
        {
			if (index >= array.Length)
			{
				return array[index % array.Length];
			}
			else if (index < 0)
            {
				return array[index % array.Length + array.Length];
            }

			return array[index];
        }

		public static T[,] Resize2DArray<T>(T[,] original, int rows, int columns)
        {
			T[,] newArray = new T[rows, columns];
			int minRows = (int)ScrapMath.Min(rows, original.GetLength(0));
			int minColumns = (int)ScrapMath.Min(columns, original.GetLength(1));
			for (int i = 0; i < minRows; i++)
            {
				for (int j = 0; j < minColumns; j++)
                {
					newArray[i, j] = original[i, j];
                }
            }

			return newArray;
        }
	}
}
