using System;

using Microsoft.Xna.Framework;

namespace ScrapBox.SMath
{
	public static class ScrapMath
	{
		public static double Length(ScrapVector v)
		{
			return System.Math.Sqrt(v.X * v.X + v.Y * v.Y);
		}

		public static double Distance(ScrapVector a, ScrapVector b)
		{
			ScrapVector delta = a - b;
			return Length(delta);
		}

		public static ScrapVector Normalize(ScrapVector v)
		{
			double length = Length(v);
			v /= length;
			return v;
		}

		public static double Dot(ScrapVector a, ScrapVector b)
		{
			return a.X * b.X + a.Y * b.Y;
		}

		public static double Cross(ScrapVector a, ScrapVector b)
		{
			return a.X * b.Y - a.Y * b.X;
		}

		public static ScrapVector RotatePoint(ScrapVector p, ScrapVector o, float theta)
		{
			return new ScrapVector(
					Math.Cos(theta) * (p.X - o.X) - Math.Sin(theta) * (p.Y-o.Y) + o.X,
					Math.Sin(theta) * (p.X - o.X) + Math.Cos(theta) * (p.Y-o.Y) + o.Y);
		}

        public static void ProjectVerticies(ScrapVector[] verts, ScrapVector axis, out double min, out double max)
        {
            min = float.MaxValue;
            max = float.MinValue;

            for (int i = 0; i < verts.Length; i ++)
            {
                ScrapVector v = verts[i];
                double projection = ScrapMath.Dot(v, axis);
                
                if (projection < min)
                    min = projection;
                
                if (projection > max)
                    max = projection;
            }
        }

		public static ScrapVector FindArithmeticMean(ScrapVector[] verts)
		{
			ScrapVector sum = ScrapVector.Zero;
			foreach (ScrapVector vert in verts)
			{
				sum += vert;
			}

			return sum / verts.Length;
		}
	}
}
