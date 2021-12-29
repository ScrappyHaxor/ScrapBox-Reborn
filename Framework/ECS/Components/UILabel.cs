//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework;

//using ScrapBox.Framework.Managers;
//using ScrapBox.Framework.Services;
//using ScrapBox.Framework.Math;
//using ScrapBox.Framework.Scene;

//namespace ScrapBox.Framework.ECS.Components
//{
//    public class UILabel : IComponent
//    {
//        public Entity Owner { get; set; }
//        public bool IsAwake { get; set; }
//        public Transform Transform { get; set; }

//        public string Label { get; set; }
//        public SpriteFont Font { get; set; }
//        public Color TextColor { get; set; }

//        public UILabel()
//        {

//        }

//        public void Awake()
//        {
//            Transform = Owner.GetComponent<Transform>();
//            if (Transform == null)
//            {
//                LogService.Log("Sprite2D", "Awake", "Missing dependency. Requires transform component to work.", Severity.ERROR);
//                return;
//            }

//            if (!Transform.IsAwake)
//            {
//                LogService.Log("Sprite2D", "Awake", "Transform component is not awake... Aborting...", Severity.ERROR);
//                return;
//            }

//            IsAwake = true;
//        }

//        public virtual void Sleep()
//        {
//            IsAwake = false;
//        }

//        public void Update(double dt)
//        {

//        }

//        public void Draw(Camera camera)
//        {
//            if (Font == null)
//                return;

//            Renderer.RenderText(Font, Label, Transform.Position - new ScrapVector(Font.MeasureString(Label)/2), TextColor);
//        }
//    }
//}
