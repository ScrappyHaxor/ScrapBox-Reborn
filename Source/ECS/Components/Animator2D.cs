using System;
using System.Timers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Managers;
using ScrapBox.SMath;

namespace ScrapBox.ECS.Components
{
	public class Animator2D : IComponent
	{
		public Entity Owner { get; set; }
		public bool IsAwake { get; set; }
		
		public Sprite2D Sprite { get; set; }

		public Rectangle CurrentCell { get { return cells[currentColumn, currentRow]; } }

		public int Columns { get; set; }
		public int Rows { get; set; }
		public int ColumnOffset { get; set; }
		public int RowOffset { get; set; }
		public float Threshold { get; set; }
		public (int, int) CellSize { get; set; }

		private Timer cycleTimer;
		private Rectangle[,] cells;
		private int currentColumn;
		private int currentRow;

		public Animator2D()
			: base()
		{
			cycleTimer = new Timer();
			cycleTimer.Elapsed += CycleAnimation;
		}

		protected virtual void ReadAnimation()
		{
			cells = new Rectangle[Columns, Rows];
			cycleTimer.Interval = Threshold;

			int currentColumnIndex = 0;
			int currentRowIndex = 0;
			while (currentRowIndex < Rows)
			{
				cells[currentColumnIndex, currentRowIndex] = 
					new Rectangle(
					(currentColumnIndex + ColumnOffset) * CellSize.Item1,
					(currentRowIndex + RowOffset) * CellSize.Item2, CellSize.Item1, CellSize.Item2);

				currentColumnIndex++;

				if (currentColumnIndex == Columns)
				{
					currentColumnIndex = 0;
					currentRowIndex++;
				}
			}
		}

		protected virtual void CycleAnimation(object o, EventArgs e)
		{
			currentColumn++;
			if (currentColumn == Columns)
			{
				currentColumn = 0;
				currentRow++;
			}

			if (currentRow == Rows)
			{
				currentRow = 0;
			}
		}

		public virtual void StartAnimating()
		{
			CycleAnimation(null, null);
			cycleTimer.Start();
		}

		public virtual void StopAnimating()
		{
			cycleTimer.Stop();
		}

		public virtual void ResetAnimation()
		{
			currentColumn = 0;
			currentRow = 0;
		}

		public virtual void Awake()
		{
			Sprite = Owner.GetComponent<Sprite2D>();

			if (Sprite == null)
			{
				LogManager.Log(new LogMessage("Animator2D", "Missing dependency. Requires Sprite2D component to work.", LogMessage.Severity.ERROR));
				return;
			}

			if (!Sprite.IsAwake)
			{
				LogManager.Log(new LogMessage("Animator2D", "Sprite2D component is not awake.. Aborting...", LogMessage.Severity.ERROR));
				return;
			}

			ReadAnimation();	
			IsAwake = true;
		}

		public virtual void Update(GameTime gameTime)
		{
			if (!IsAwake)
				return;
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			if (!IsAwake)
				return;

			Renderer2D.RenderSprite(Sprite.Texture, Sprite.Transform.Position, 
					Sprite.Transform.Dimensions, Sprite.Transform.Rotation, cells[currentColumn, currentRow], 
					Sprite.TintColor, Sprite.Effects);
		}
	}
}
