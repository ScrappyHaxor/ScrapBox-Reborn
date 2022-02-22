using System.Collections.Generic;

using ScrapBox.Framework.Services;
using ScrapBox.Framework.Level;
using ScrapBox.Framework.Managers;

namespace ScrapBox.Framework.ECS
{
	public abstract class Entity
	{
		public abstract string Name { get; }

		public int ID { get; set; }
		public bool IsAwake { get; set; }

		private readonly List<Component> register = new List<Component>();

		//The entire entity component system is in need of optimization. Too many typeof and gettype
	
		~Entity()
        {
			foreach (Component component in register)
            {
				component.Sleep();
            }

			register.Clear();
        }

		protected bool Dependency<T>(out T entity, bool optional = false) where T : Entity
		{
			entity = default;

			if (!WorldManager.HasEntity<T>())
			{
				if (!optional)
				{
					LogService.Log(Name, "Dependency", $"Missing dependency. Requires type {typeof(T)} to work.", Severity.ERROR);
				}

				return false;
			}

			entity = WorldManager.GetEntity<T>();
			if (!entity.IsAwake)
			{
				if (!optional)
				{
					LogService.Log(Name, "Dependency", $"Dependency type {typeof(T)} is not awake.", Severity.ERROR);
				}

				return false;
			}

			return true;
		}

		public void RegisterComponent<T>(T component) where T : Component
		{
			if (HasComponent<T>())
            {
				LogService.Log("ECS", "RegisterComponent", "Component already exists.", Severity.ERROR);
				return;
			}
				
			register.Add(component);
			component.Owner = this;
		}

		public void PurgeComponent<T>(T component) where T : Component
        {
			if (!HasComponent<T>())
            {
				LogService.Log("ECS", "PurgeComponent", "Component doesn't exist.", Severity.ERROR);
				return;
			}

			register.Remove(component);	
        }

		public bool HasComponent<T>() where T : Component
		{
			foreach (Component component in register)
			{
				if (component.GetType().Equals(typeof(T)) || component.GetType().IsSubclassOf(typeof(T)))
					return true;
			}

			return false;
		}

		public T GetComponent<T>() where T : Component
		{
			foreach (Component component in register)
			{
				if (component.GetType().Equals(typeof(T)) || component.GetType().IsSubclassOf(typeof(T)))
					return (T)component;
			}
			
			LogService.Log("ECS", "GetComponent", "Tried to get non-existant component.", Severity.WARNING);
			return default;
		}

		public virtual void Awake() 
		{
			if (IsAwake)
				return;

			foreach (Component component in register)
			{
				component.Awake();
			}

			WorldManager.RegisterEntity(this);
			IsAwake = true;
		}

		public virtual void Sleep()
        {
			if (!IsAwake)
				return;

			foreach (Component component in register)
            {
				component.Sleep();
            }

			WorldManager.PurgeEntity(this);
			IsAwake = false;
        }

		public virtual void Update(double dt) 
		{ 

		}

		public virtual void Draw(Camera mainCamera)
        {

        }
	}
}
