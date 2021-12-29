using Microsoft.Xna.Framework;

namespace ScrapBox.Framework.Math
{
	public readonly struct ScrapVector
	{
		public static readonly ScrapVector Zero = new ScrapVector(0, 0);
		public static readonly ScrapVector One = new ScrapVector(1, 1);

		public readonly double X;
		public readonly double Y;
		
		public ScrapVector(double x)
		{
			if (x == -0)
            {
				x = 0;
            }

			X = x;
			Y = x;
		}

		public ScrapVector(double x, double y)
		{
			if (x == -0)
            {
				x = 0;
            }

			if (y == -0)
            {
				y = 0;
            }

			X = x;
			Y = y;
		}

		public ScrapVector(Vector2 v)
        {
			if (v.X == -0)
            {
				v.X = 0;
            }

			if (v.Y == -0)
            {
				v.Y = 0;
            }

			X = v.X;
			Y = v.Y;
        }

		public static ScrapVector Parse(string input)
        {
			string[] components = input.Split(",");
			return new ScrapVector(double.Parse(components[0]), double.Parse(components[1]));
        }

		public static ScrapVector operator+(ScrapVector a, ScrapVector b)
		{
			return new ScrapVector(a.X + b.X, a.Y + b.Y);
		}

		public static ScrapVector operator-(ScrapVector a, ScrapVector b)
		{
			return new ScrapVector(a.X - b.X, a.Y - b.Y);
		}

		public static ScrapVector operator-(ScrapVector v)
		{
			return new ScrapVector(-v.X, -v.Y);
		}


		public static ScrapVector operator*(ScrapVector a, ScrapVector b)
		{
			return new ScrapVector(a.X * b.X, a.Y * b.Y);
		}

		public static ScrapVector operator/(ScrapVector a, ScrapVector b)
		{
			return new ScrapVector(a.X / b.X, a.Y / b.Y);
		}

		public static ScrapVector operator*(ScrapVector a, double s)
		{
			return new ScrapVector(a.X * s, a.Y * s);
		}

		public static ScrapVector operator/(ScrapVector a, double s)
		{
			return new ScrapVector(a.X / s, a.Y / s);
		}

		public static ScrapVector operator*(double s, ScrapVector a)
		{
			return new ScrapVector(a.X * s, a.Y * s);
		}

		public static ScrapVector operator/(double s, ScrapVector a)
		{
			return new ScrapVector(a.X / s, a.Y / s);
		}

		public static bool operator==(ScrapVector a, ScrapVector b)
		{
			return a.X == b.X && a.Y == b.Y;
		}

		public static bool operator!=(ScrapVector a, ScrapVector b)
		{
			return a.X != b.X || a.Y != b.Y;
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj.GetHashCode() == GetHashCode();
		}

		public bool Equal(ScrapVector other)
		{
			return X == other.X && Y == other.Y;
		}

		public override string ToString()
		{
			return $"{X},{Y}";
		}

		public static ScrapVector Transform(ScrapVector position, Matrix matrix)
		{
			return new ScrapVector((position.X * matrix.M11) + (position.Y * matrix.M21) + matrix.M41, (position.X * matrix.M12) + (position.Y * matrix.M22) + matrix.M42);
		}

		public static implicit operator Vector2(ScrapVector v) => new Vector2((float)v.X, (float)v.Y);
	}
}
