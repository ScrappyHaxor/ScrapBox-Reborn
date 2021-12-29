using ScrapBox.Framework.Services;

namespace ScrapBox.Framework.ECS
{
	public abstract class Component
	{
		public abstract string Name { get; }

		public Entity Owner { get; set; }
		public bool IsAwake { get; set; }

		protected bool Dependency<T>(out T comp, bool optional = false) where T : Component
		{
			comp = default;

			if (!Owner.HasComponent<T>())
			{
				if (!optional)
                {
                    LogService.Log(Name, "Dependency", $"Missing dependency. Requires type {typeof(T)} to work.", Severity.ERROR);
				}
				
				return false;
			}

			comp = Owner.GetComponent<T>();
			if (!comp.IsAwake)
            {
				if (!optional)
                {
                    LogService.Log(Name, "Dependency", $"Dependency type {typeof(T)} is not awake.", Severity.ERROR);
				}
					
				return false;
            }

			return true;
        }

		public abstract void Awake();
		public abstract void Sleep();
	}
}
