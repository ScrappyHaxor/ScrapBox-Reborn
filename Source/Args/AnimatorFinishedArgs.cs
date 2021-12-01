using System;
using System.Collections.Generic;
using System.Text;

namespace ScrapBox.Args
{
    public class AnimatorFinishedArgs
    {
        public string Name { get; set; }

        public AnimatorFinishedArgs(string name)
        {
            Name = name;
        }
    }
}
