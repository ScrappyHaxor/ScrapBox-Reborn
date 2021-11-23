using Microsoft.Xna.Framework;

namespace ScrapBox.SMath
{
	//Wanted my own hyperfast lightweight implementation - Scrappy (Hampus H)
	public readonly struct ScrapVector
	{
		public static readonly ScrapVector Zero = new ScrapVector(0f, 0f);

		public readonly double X;
		public readonly double Y;
		
		public ScrapVector(double x, double y)
		{
			X = x;
			Y = y;
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
	}
}
