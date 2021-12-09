using System;

using Microsoft.Xna.Framework;

namespace ScrapBox.Framework.Math
{
	public static class ScrapMath
	{
		public static double PI { get { return System.Math.PI; } }

		public static double Deltafy(GameTime gameTime)
		{
			return gameTime.ElapsedGameTime.TotalSeconds;
		}

		public static double Length(ScrapVector v)
		{
			return System.Math.Sqrt(v.X * v.X + v.Y * v.Y);
		}

		public static double Cos(double theta)
        {
			return System.Math.Cos(theta);
        }

		public static double Floor(double d)
        {
			return System.Math.Floor(d);
        }

		public static double Ceil(double d)
        {
			return System.Math.Ceiling(d);
        }

		public static double Round(double d)
        {
			return System.Math.Round(d);
        }

		public static double Sin(double theta)
        {
			return System.Math.Sin(theta);
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

		public static ScrapVector Cross(ScrapVector v, double s)
		{
			return new ScrapVector(v.Y * s, v.X * -s);
		}

		public static ScrapVector Cross(double s, ScrapVector v)
		{
			return new ScrapVector(-s * v.Y, s * v.X);
		}

		public static ScrapVector RotatePoint(ScrapVector p, ScrapVector o, float theta)
		{
			return new ScrapVector(
					Cos(theta) * (p.X - o.X) - Sin(theta) * (p.Y-o.Y) + o.X,
					Sin(theta) * (p.X - o.X) + Cos(theta) * (p.Y-o.Y) + o.Y);
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

		public static double Min(double a, double b)
		{
			if (a < b)
			{
				return a;
			}
			else
			{
				return b;
			}
		}

		public static double Max(double a, double b)
		{
			if (a > b)
			{
				return a;
			}
			else
			{
				return b;
			}
		}

		public static double Abs(double a)
		{
			return a > 0.0 ? a : -a;
		}

		public static ScrapVector Abs(ScrapVector a)
		{
			return new ScrapVector(Abs(a.X), Abs(a.Y));
		}

		public static Random GetSeededRandom()
        {
			return new Random(Guid.NewGuid().GetHashCode());
        }

		public static double Lerp(double a, double b, double amount)
        {
			return a + (b - a) * amount;
        }

		public static ScrapVector RandomPointInCircle(double radius)
		{
			Random rand = GetSeededRandom();
			double theta = 2 * PI * rand.NextDouble();
			double u = rand.NextDouble() + rand.NextDouble();

			if (u > 1)
			{
				return new ScrapVector(radius * (2 - u) * Cos(theta), radius * (2 - u) * Sin(theta));
			}

			return new ScrapVector(radius * u * Cos(theta), radius * u * Sin(theta));
		}

		public static double RoundOnGrid(double n, double TileSize)
        {
			return Floor(((n + TileSize - 1) / TileSize)) * TileSize;
        }

		public static void Swap<T>(ref T a, ref T b)
		{
			T tmp = a;
			a = b;
			b = tmp;
		}
	}
}
