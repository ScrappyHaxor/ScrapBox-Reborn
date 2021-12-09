using System;

using ScrapBox.Framework.ECS;

namespace ScrapBox.Framework
{
    public class TriggerArgs : EventArgs
    { 
        public Collider Other { get; set; }

        public TriggerArgs(Collider other)
        {
            Other = other;
        }
    }
}
