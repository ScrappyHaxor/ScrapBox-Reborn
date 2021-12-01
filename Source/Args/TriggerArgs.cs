using System;
using System.Collections.Generic;
using System.Text;

using ScrapBox.ECS.Components;

namespace ScrapBox.Args
{
    public class TriggerArgs : EventArgs
    { 
        public ICollider Other { get; set; }

        public TriggerArgs(ICollider other)
        {
            Other = other;
        }
    }
}
