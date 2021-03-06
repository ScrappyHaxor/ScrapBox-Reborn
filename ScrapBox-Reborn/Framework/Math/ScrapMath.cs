using System;

using Microsoft.Xna.Framework;

using ScrapBox.Framework.Level;

namespace ScrapBox.Framework.Math
{
	public static class ScrapMath
	{
		public static double PI { get { return System.Math.PI; } }
		public static double PI2 { get { return PI * 2; } }

		private const double SMALL = 1 / double.MaxValue;

		public static double Deltafy(GameTime gameTime)
		{
			return gameTime.ElapsedGameTime.TotalSeconds;
		}

		public static double Length(ScrapVector v)
		{
			return System.Math.Sqrt(v.X * v.X + v.Y * v.Y);
		}

		public static double Distance(ScrapVector a, ScrapVector b)
		{
			ScrapVector delta = a - b;
			return Length(delta);
		}

		public static double ToRadians(double theta)
        {
			return theta * PI / 180;
        }

		public static double Cos(double theta)
        {
			return System.Math.Cos(theta);
        }

		public static double Atan(double theta)
        {
			return System.Math.Atan(theta);
        }

		public static double Atan2(double a, double b)
        {
			return System.Math.Atan2(a, b);
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

		public static double Clamp(double d, double min, double max)
        {
			return System.Math.Clamp(d, min, max);
        }

		public static ScrapVector Floor(ScrapVector v)
		{
			return new ScrapVector(System.Math.Floor(v.X), System.Math.Floor(v.Y));
		}

		public static ScrapVector Ceil(ScrapVector v)
		{
			return new ScrapVector(System.Math.Ceiling(v.X), System.Math.Ceiling(v.Y));
		}

		public static ScrapVector Round(ScrapVector v)
        {
			return new ScrapVector(System.Math.Round(v.X), System.Math.Round(v.Y));
        }

		public static double Sqrt(double d)
        {
			return System.Math.Sqrt(d);
        }

		public static ScrapVector Sqrt(ScrapVector v)
        {
			return new ScrapVector(System.Math.Sqrt(v.X), System.Math.Sqrt(v.Y));
        }

		public static double Sin(double theta)
        {
			return System.Math.Sin(theta);
        }

		public static double Tan(double theta)
        {
			return System.Math.Tan(theta);
        }

		public static ScrapVector Normalize(ScrapVector v)
		{
			double length = Length(v);

			if (length == 0)
            {
				length = SMALL;
            }

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

		public static ScrapVector RotatePoint(ScrapVector p, ScrapVector o, double theta)
		{
			return new ScrapVector(
					Cos(theta) * (p.X - o.X) - Sin(theta) * (p.Y-o.Y) + o.X,
					Sin(theta) * (p.X - o.X) + Cos(theta) * (p.Y-o.Y) + o.Y);
		}

		public static ScrapVector RotatePoint(ScrapVector p, double theta)
        {
			return new ScrapVector(
				Cos(theta) * p.X - Sin(theta) * p.Y, 
				Sin(theta) * p.X + Cos(theta) * p.Y);
        }

        public static void ProjectVerticies(ScrapVector[] verts, ScrapVector axis, out double min, out double max)
        {
            min = float.MaxValue;
            max = float.MinValue;

			if (verts == null || verts.Length == 0)
				return;

			for (int i = 0; i < verts.Length; i ++)
            {
                ScrapVector v = verts[i];
                double projection = Dot(v, axis);
                
                if (projection < min)
                    min = projection;
                
                if (projection > max)
                    max = projection;
            }
        }

		public static void ProjectCircle(ScrapVector center, double radius, ScrapVector axis, out double min, out double max)
		{
			ScrapVector direction = Normalize(axis);
			ScrapVector radiusDirection = direction * radius;

			ScrapVector point1 = center + radiusDirection;
			ScrapVector point2 = center - radiusDirection;

			min = Dot(point1, axis);
			max = Dot(point2, axis);

			if (min > max)
			{
				double temp = min;
				min = max;
				max = temp;

				//Swap(ref min, ref max);
			}
		}

		public static int ClosestPointPolygon(ScrapVector center, ScrapVector[] verts)
		{
			int result = -1;
			double minDistance = float.MaxValue;

			for (int i = 0; i < verts.Length; i++)
			{
				ScrapVector vert = verts[i];
				double distance = Distance(vert, center);
				if (distance < minDistance)
				{
					minDistance = distance;
					result = i;
				}
			}

			return result;
		}

		public static ScrapVector FindArithmeticMean(ScrapVector[] verts)
		{
			if (verts == null || verts.Length == 0)
				return default;

			ScrapVector sum = ScrapVector.Zero;
			foreach (ScrapVector vert in verts)
			{
				sum += vert;
			}

			return sum / verts.Length;
		}

		public static ScrapVector ScreenToWorldCoordinates(ScrapVector v, Camera camera)
        {
			Matrix inverseTransformMatrix = Matrix.Invert(camera.TransformationMatrix);
			return ScrapVector.Transform(v, inverseTransformMatrix);
		}

		public static void GetCenteredRectangle(Rectangle source, out ScrapVector position, out ScrapVector dimensions)
        {
			position = new ScrapVector(source.X + source.Width / 2, source.Y + source.Height / 2);
			dimensions = new ScrapVector(source.Width, source.Height);
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
			if (TileSize == 0)
				return default;

			return Floor(((n + TileSize - 1) / TileSize)) * TileSize;
        }

		public static double SnapOnGrid(double n, double gridSize)
        {
			return ScrapMath.Round(n / gridSize) * gridSize;
		}
	}
}
