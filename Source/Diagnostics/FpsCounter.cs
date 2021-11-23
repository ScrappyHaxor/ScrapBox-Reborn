using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScrapBox.Diagnostics
{
	public class FpsCounter
	{
		public int TotalFrames { get; internal set; }
		public double ElapsedTime { get; internal set; }
		public int Fps { get; internal set; }

		public SpriteFont debugFont { get; set; }


		public FpsCounter(SpriteFont debugFont = null)
		{
			this.debugFont = debugFont;
		}

		public void Update(GameTime gameTime)
		{
			TotalFrames++;
			ElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;

			if (ElapsedTime >= 1000)
			{
				Fps = TotalFrames;
				TotalFrames = 0;
				ElapsedTime = 0;
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (debugFont == null)
				return;

			string fps = $"FPS: {Fps}";
			spriteBatch.Begin();
			spriteBatch.DrawString(debugFont, fps, Vector2.Zero, Color.Black);
			spriteBatch.End();
		}
	}
}
