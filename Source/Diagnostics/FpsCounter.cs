using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScrapBox.Diagnostics
{
	public class FpsCounter
	{
		public double Fps { get; internal set; }

		public SpriteFont debugFont { get; set; }


		public FpsCounter(SpriteFont debugFont = null)
		{
			this.debugFont = debugFont;
		}

		public void Update(double dt)
		{
			
		}

		public void Draw(SpriteBatch spriteBatch, double dt)
		{
			Fps = 1 / dt;
			if (debugFont == null)
				return;

			string fps = $"FPS: {Fps}";
			spriteBatch.Begin();
			spriteBatch.DrawString(debugFont, fps, Vector2.Zero, Color.Black);
			spriteBatch.End();
		}
	}
}
