using Microsoft.Xna.Framework;

namespace ScrapBox.Math
{
	public static class ScrapConverter
	{
		public static Vector2 ToMonoVector2(ScrapVector v)
		{
			return new Vector2(v.X, v.Y);
		}
	}
}
