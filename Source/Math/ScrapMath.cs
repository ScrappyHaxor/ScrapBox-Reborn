using Microsoft.Xna.Framework;

namespace ScrapBox.Math
{
	public static class ScrapMath
	{
		public static float Length(Vector2 v)
		{
			return (float)System.Math.Sqrt(v.X * v.X + v.Y * v.Y);
		}

		public static float Distance(Vector2 a, Vector2 b)
		{
			Vector2 delta = a - b;
			return Length(delta);
		}

		public static Vector2 Normalize(Vector2 v)
		{
			float length = Length(v);
			v /= length;
			return v;
		}

		public static float Dot(Vector2 a, Vector2 b)
		{
			return a.X * b.X + a.Y * b.Y;
		}

		public static float Cross(Vector2 a, Vector2 b)
		{
			return a.X * b.Y - a.Y * b.X;
		}
	}
}
