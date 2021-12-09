using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.Managers;
using ScrapBox.Framework.Scene;

namespace ScrapBox.Framework.ECS
{
	public class Entity
	{
		public int ID { get; set; }
		public bool IsAwake { get; set; }

		private readonly List<IComponent> componentRegister = new List<IComponent>();
	
		public T AddComponent<T>(T component) where T : IComponent
		{
			if (componentRegister.Contains(component))
				LogManager.Log(new LogMessage("ECS", "Component already exists. Expect undefined behaviour from here.", LogMessage.Severity.WARNING));

			componentRegister.Add(component);
			component.Owner = this;
			return component;
		}

		public T GetComponent<T>() where T : IComponent 
		{
			foreach (IComponent component in componentRegister)
			{
				if (component.GetType().Equals(typeof(T)) ||
					component.GetType().IsSubclassOf(typeof(T)))
					return (T)component;
			}
			
			LogManager.Log(new LogMessage("ECS", "Tried to get non-existant component. Use HasComponent if the component may not be attached.", LogMessage.Severity.WARNING));
			return default;
		}

		public bool HasComponent<T>() where T : IComponent
		{
			foreach (IComponent component in componentRegister)
			{
				if (component.GetType().Equals(typeof(T)) || 
					component.GetType().IsSubclassOf(typeof(T)))
					return true;
			}

			return false;
		}
		
		public virtual void Awake() 
		{ 
			foreach (IComponent component in componentRegister)
			{
				component.Awake();
			}

			IsAwake = true;
		}

		public virtual void Sleep()
        {
			foreach (IComponent component in componentRegister)
            {
				component.Sleep();
            }

			IsAwake = false;
        }

		public virtual void Update(double dt) 
		{ 
			if (!IsAwake) return;

			foreach (IComponent component in componentRegister)
			{
				component.Update(dt);
			}
		}

		public virtual void Draw(SpriteBatch spriteBatch, Camera camera) 
		{ 
			if (!IsAwake) return; 

			foreach (IComponent component in componentRegister)
			{
				component.Draw(spriteBatch, camera);
			}
		} 
	}
}
