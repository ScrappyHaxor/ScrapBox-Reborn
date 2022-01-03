using System;
using System.Collections.Generic;
using System.Text;

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
	}
}
