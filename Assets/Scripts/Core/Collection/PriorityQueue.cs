using System;
using System.Collections.Generic;

namespace Core.Collection
{
    public sealed class PriorityQueue<T>
    {
        private readonly List<Entry> _items = new();
        
        public int Count => _items.Count;

        public void Enqueue(T item, int priority)
        {
            _items.Add(new Entry(item, priority));
            SiftUp(_items.Count - 1);
        }

        public T Dequeue()
        {
            if (_items.Count == 0)
                throw new InvalidOperationException("queue is empty");

            T result = _items[0].Item;
            
            int lastIndex = _items.Count - 1;

            if (lastIndex == 0)
            {
                _items.RemoveAt(0);
                return result;
            }
            
            _items[0] = _items[lastIndex];
            _items.RemoveAt(lastIndex);

            SiftDown(0);
            
            return result;
        }

        public void Clear()
        {
            _items.Clear();
        }

        private void SiftUp(int index)
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;

                if (_items[index].Priority >=
                    _items[parentIndex].Priority)
                {
                    break;
                }
                
                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        private void SiftDown(int index)
        {
            int count = _items.Count;

            while (true)
            {
                int leftIndex = index * 2 + 1;
                int rightIndex = index * 2 + 2;
                int smallestIndex = index;

                if (leftIndex < count &&
                    _items[leftIndex].Priority <
                    _items[smallestIndex].Priority)
                {
                    smallestIndex = leftIndex;
                }
                
                if (rightIndex < count &&
                    _items[rightIndex].Priority <
                    _items[smallestIndex].Priority)
                {
                    smallestIndex = leftIndex;
                }
                
                if (smallestIndex == index)
                    break;
                
                Swap(index, smallestIndex);
                index = smallestIndex;
            }
        }

        private void Swap(int a, int b)
        {
            (_items[a], _items[b]) =  (_items[b], _items[a]);
        }

        private readonly struct Entry
        {
            public T Item { get; }
            public int Priority { get; }

            public Entry(T item, int priority)
            {
                Item = item;
                Priority = priority;
            }
        }
    }
}