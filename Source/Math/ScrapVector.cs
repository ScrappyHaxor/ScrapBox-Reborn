using Microsoft.Xna.Framework;

namespace ScrapBox.SMath
{
	public readonly struct ScrapVector
	{
		public static readonly ScrapVector Zero = new ScrapVector(0, 0);
		public static readonly ScrapVector One = new ScrapVector(1, 1);

		public readonly double X;
		public readonly double Y;
		
		public ScrapVector(double x)
		{
			X = x;
			Y = x;
		}

		public ScrapVector(double x, double y)
		{
			X = x;
			Y = y;
		}

		public ScrapVector(Vector2 v)
        {
			X = v.X;
			Y = v.Y;
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
			return (X + Y).GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj.GetHashCode() == GetHashCode();
		}

		public bool Equal(ScrapVector other)
		{
			return X == other.X && Y == other.Y;
		}

		public Vector2 ToMono()
		{
			return new Vector2((float)X, (float)Y);
		}

		public override string ToString()
		{
			return $"{X}, {Y}";
		}

		public static ScrapVector Transform(ScrapVector position, Matrix matrix)
		{
			return new ScrapVector((position.X * matrix.M11) + (position.Y * matrix.M21) + matrix.M41, (position.X * matrix.M12) + (position.Y * matrix.M22) + matrix.M42);
		}	
	}
}
