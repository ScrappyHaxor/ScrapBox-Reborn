using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Managers;

namespace ScrapBox.ECS.Components
{
	public class Sprite2D : IComponent
	{
		public Entity Owner { get; set; }
		public bool IsAwake { get; set; }

		public Transform Transform { get; set; }
		public Texture2D Texture { get; set; }
		public Rectangle? SourceRectangle { get; set; }
		public Color TintColor { get; set; }
		public SpriteEffects Effects { get; set; }
		public float Depth { get; set; }

		protected bool animatorAttached;

		public Sprite2D()
		{
			TintColor = Color.White;
		}

		public virtual void Awake()
		{
			Transform = Owner.GetComponent<Transform>();
			if (Transform == null)
			{
				LogManager.Log(new LogMessage("Sprite2D", "Missing dependency. Requires transform component to work.", LogMessage.Severity.ERROR));
				return;
			}

			if (!Transform.IsAwake)
			{
				LogManager.Log(new LogMessage("Sprite2D", "Transform component is not awake... Aborting...", LogMessage.Severity.ERROR));
				return;
			}

			animatorAttached = Owner.HasComponent<Animator2D>();

			IsAwake = true;
		}

		public virtual void Update(GameTime gameTime)
		{		
			if (!IsAwake)
				return;
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			if (!IsAwake || animatorAttached)
				return;			
				
			Renderer2D.RenderSprite(this);	
		}
	}
}
