using System;
using System.Timers;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Managers;
using ScrapBox.SMath;
using ScrapBox.Scene;
using ScrapBox.Utils;
using ScrapBox.Args;

namespace ScrapBox.ECS.Components
{
	public class Animator2D : IComponent
	{
		public EventHandler<AnimatorFinishedArgs> Finished { get; set; }

		public Entity Owner { get; set; }
		public bool IsAwake { get; set; }
		
		public Sprite2D Sprite { get; set; }

		public bool Playing { get { return enabled; } }

		public (int, int) CellSize { get; set; }

		private Timer cycleTimer;
		protected bool enabled;
		private AnimationClip clip;
		private Rectangle[,] cells;
		private int currentColumn;
		private int currentRow;

		private Dictionary<string, AnimationClip> Clips;

		public Animator2D()
			: base()
		{
			cycleTimer = new Timer();
			cycleTimer.Elapsed += CycleAnimation;
			Clips = new Dictionary<string, AnimationClip>();
		}

		public virtual void RegisterClip(string name, AnimationClip clip)
        {
			clip.Name = name;
			Clips.Add(name, clip);
        }

		public virtual void SwapClip(string name)
        {
			ResetAnimation();
			clip = Clips.GetValueOrDefault(name);
			cycleTimer.Interval = clip.Speed;
        }

		public virtual bool ClipLoaded(string name)
        {
			return name == clip.Name;
        }

		protected virtual void ReadSheet()
		{
			int MaxColumns = Sprite.Texture.Width / CellSize.Item1;
			int MaxRows = Sprite.Texture.Height / CellSize.Item2;

			cells = new Rectangle[MaxColumns, MaxRows];

			int currentColumnIndex = 0;
			int currentRowIndex = 0;
			while (currentRowIndex < MaxRows)
			{
				cells[currentColumnIndex, currentRowIndex] = 
					new Rectangle(
					currentColumnIndex * CellSize.Item1,
					currentRowIndex * CellSize.Item2, 
					CellSize.Item1, CellSize.Item2);

				currentColumnIndex++;

				if (currentColumnIndex == MaxColumns)
				{
					currentColumnIndex = 0;
					currentRowIndex++;
				}
			}
		}

		protected virtual void CycleAnimation(object o, EventArgs e)
		{
			currentColumn++;
			if (currentColumn == clip.Columns)
			{
				currentColumn = 0;
				currentRow++;
			}

			if (currentRow == clip.Rows)
			{
				if (!clip.Looped)
                {
					cycleTimer.Stop();
					currentColumn = clip.Columns-1;
					enabled = false;
					Finished?.Invoke(this, new AnimatorFinishedArgs(clip.Name));
                }

				currentRow = 0;
			}
		}

		public virtual void StartAnimating()
		{
			enabled = true;
			CycleAnimation(null, null);
			cycleTimer.Start();
		}

		public virtual void StopAnimating()
		{
			enabled = false;
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

			ReadSheet();	
			IsAwake = true;
		}

		public virtual void Update(double dt)
		{
			if (!IsAwake)
				return;
		}

		public virtual void Draw(SpriteBatch spriteBatch, Camera camera)
		{
			if (!IsAwake || clip == null)
				return;

			Sprite.SourceRectangle = cells[currentColumn + clip.ColumnOffset, currentRow + clip.RowOffset];
		}
	}
}
