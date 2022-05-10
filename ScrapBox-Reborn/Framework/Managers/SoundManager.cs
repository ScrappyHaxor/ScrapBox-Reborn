using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace ScrapBox.Framework.Managers
{
    //temporary class
    public static class SoundManager
    {
        public static SoundEffectInstance Jump { get; private set; }
        public static SoundEffectInstance Collision { get; private set; }

        public static void LoadSound(ContentManager content)
        {
            Jump = content.Load<SoundEffect>("jumping").CreateInstance();
            Jump.Volume = 0.1f;

            Collision = content.Load<SoundEffect>("collision").CreateInstance();
            Collision.Volume = 0.1f;
        }
    }
}
