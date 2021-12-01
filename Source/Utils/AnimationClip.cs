using System;
using System.Collections.Generic;
using System.Text;

namespace ScrapBox.Utils
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
}
