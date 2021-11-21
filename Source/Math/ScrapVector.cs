

namespace ScrapBox.Math
{
	//Wanted my own hyperfast lightweight implementation - Scrappy (Hampus H)
	public readonly struct ScrapVector
	{
		public static readonly ScrapVector Zero = new ScrapVector(0f, 0f);

		public readonly float X;
		public readonly float Y;
		
		public ScrapVector(float x, float y)
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

		public static ScrapVector operator*(ScrapVector a, float s)
		{
			return new ScrapVector(a.X * s, a.Y * s);
		}

		public static ScrapVector operator/(ScrapVector a, float s)
		{
			return new ScrapVector(a.X / s, a.Y / s);
		}

		public bool Equal(ScrapVector other)
		{
			return X == other.X && Y == other.Y;
		}

		public override string ToString()
		{
			return $"{X}, {Y}";
		}
	}
}
