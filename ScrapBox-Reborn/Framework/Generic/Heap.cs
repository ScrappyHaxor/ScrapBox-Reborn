using System;
using System.Collections.Generic;
using System.Text;

namespace ScrapBox.Framework.Generic
{
    public class Heap<T> where T : IHeapItem<T>
    {
        private readonly T[] contents;
        private int contentCount;

        public int Count { get { return contentCount; } }

        public Heap(int maxSize)
        {
            contents = new T[maxSize];
        }

        private void DeepSwap(T itemA, T itemB)
        {
            contents[itemA.HeapIndex] = itemB;
            contents[itemB.HeapIndex] = itemA;
            int temp = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = temp;
        }

        private void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;
            while (true)
            {
                T parent = contents[parentIndex];
                if (item.CompareTo(parent) > 0)
                {
                    DeepSwap(item, parent);
                }
                else
                {
                    break;
                }

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        private void SortDown(T item)
        {
            while (true)
            {
                int childIndexLeft = item.HeapIndex * 2 + 1;
                int childIndexRight = item.HeapIndex * 2 + 2;
                if (childIndexLeft < contentCount)
                {
                    int swapIndex = childIndexLeft;
                    if (childIndexRight < contentCount && contents[childIndexLeft].CompareTo(contents[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }

                    if (item.CompareTo(contents[swapIndex]) < 0)
                    {
                        DeepSwap(item, contents[swapIndex]);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        public void Add(T item)
        {
            item.HeapIndex = contentCount;
            contents[contentCount] = item;
            SortUp(item);
            contentCount++;
        }

        public T RemoveFirst()
        {
            T item = contents[0];
            contentCount--;
            contents[0] = contents[contentCount];
            contents[0].HeapIndex = 0;
            SortDown(contents[0]);

            return item;
        }

        public void Update(T item)
        {
            SortUp(item);
        }

        public bool Contains(T item)
        {
            return Equals(contents[item.HeapIndex], item);
        }
    }
}
