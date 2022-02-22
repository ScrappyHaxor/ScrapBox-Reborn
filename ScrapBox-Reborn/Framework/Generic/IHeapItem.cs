using System;
using System.Collections.Generic;
using System.Text;

namespace ScrapBox.Framework.Generic
{
    public interface IHeapItem<T> : IComparable<T>
    { 
        int HeapIndex { get; set; }
    }
}
