using System;
using System.Timers;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using ScrapBox.Framework.Services;

namespace ScrapBox.Framework.ECS.Components
{
	public class AnimationClip
	{
		public string Name { get; set; }
		public int Columns { get; set; }
		public int Rows { get; set; }
		public int Speed { get; set; }

		public int ColumnOffset { get; set; }
		public int RowOffset { get; set; }

		public bool Looped { get; set; }

		public AnimationClip(int columns, int rows, int speed, int columnOffset = 0, int rowOffset = 0, bool looped = true)
		{
			Columns = columns;
			Rows = rows;
			Speed = speed;
			ColumnOffset = columnOffset;
			RowOffset = rowOffset;
			Looped = looped;
		}
	}

	public class AnimatorFinishedArgs
	{
		public string Name { get; set; }

		public AnimatorFinishedArgs(string name)
		{
			Name = name;
		}
	}

	public class Animator2D : Component
	{
        public override string Name => "Animator2D";

        public EventHandler<AnimatorFinishedArgs> Finished { get; set; }

		public Sprite2D Sprite;

		public bool Playing { get { return enabled; } }

		public (int, int) CellSize { get; set; }

		private readonly Timer cycleTimer;
		private bool enabled;
		private AnimationClip clip;
		private Rectangle[,] cells;
		private int currentColumn;
		private int currentRow;

		private readonly Dictionary<string, AnimationClip> Clips;

		public Animator2D()
			: base()
		{
			cycleTimer = new Timer();
			cycleTimer.Elapsed += CycleAnimation;
			Clips = new Dictionary<string, AnimationClip>();
		}

		public void RegisterClip(string name, AnimationClip clip)
        {
			clip.Name = name;
			Clips.Add(name, clip);
        }

		public void SwapClip(string name)
        {
			ResetAnimation();
			clip = Clips.GetValueOrDefault(name);
			cycleTimer.Interval = clip.Speed;
        }

		public bool ClipLoaded(string name)
        {
			return name == clip.Name;
        }

		protected void ReadSheet()
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

		protected void CycleAnimation(object o, EventArgs e)
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

			Sprite.SourceRectangle = cells[currentColumn + clip.ColumnOffset, currentRow + clip.RowOffset];
		}

		public void StartAnimating()
		{
			enabled = true;
			CycleAnimation(null, null);
			cycleTimer.Start();
		}

		public void StopAnimating()
		{
			enabled = false;
			cycleTimer.Stop();
		}

		public void ResetAnimation()
		{
			currentColumn = 0;
			currentRow = 0;
		}

		public override void Awake()
		{
			if (IsAwake)
				return;

			bool success = Dependency(out Sprite);
			if (!success)
				return;

			if (clip == null)
            {
				LogService.Log(Name, "Awake", "No animation clip loaded.", Severity.ERROR);
				return;
            }

			ReadSheet();	
			IsAwake = true;
		}

		public override void Sleep()
        {
			if (!IsAwake)
				return;

			IsAwake = false;
        }
	}
}
