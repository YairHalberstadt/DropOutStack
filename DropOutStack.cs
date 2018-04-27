using System;
using System.Collections;
using System.Collections.Generic;

namespace Se.Util.DataStructures
{
	/// <summary>
	/// A stack which removes oldest object when full. Useful for undo implementation.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DropOutStack<T> : IEnumerable<T>
	{
		private readonly T[] _items;
		private readonly int _capacity;
		private int _top = 0;
		public int Count { get; private set; }

		public bool IsAtCapacity => Count == _capacity;

		public bool IsEmpty => Count == 0;

		public DropOutStack(int capacity)
		{
			_capacity = capacity;
			_items = new T[capacity];
		}

		/// <summary>
		/// Pushes the item to the top of the stack, overwriting the oldest item if stack is at capacity
		/// </summary>
		/// <param name="item"></param>
		public void Push(T item)
		{
			_items[_top] = item;
			_top = (_top + 1) % _capacity;

			if (!IsAtCapacity)
				Count++;
		}

		/// <summary>
		/// Pops the newest item off the stack and returns it
		/// </summary>
		/// <returns></returns>
		public T Pop()
		{
			if (IsEmpty)
				throw new InvalidOperationException("Stack empty.");
			Count--;

			_top = (_items.Length + _top - 1) % _capacity;
			var item = _items[_top];
			_items[_top] = default(T); // Allow garbage collector to free memory
			return item;
		}

		/// <summary>
		/// Will not push if stack is full, to avoid overwriting oldest item.
		/// </summary>
		/// <param name="item"></param>
		/// <returns>true if not at capacity (Push was successful), false otherwise.</returns>
		public bool PushIfNotAtCapacity(T item)
		{
			if (IsAtCapacity)
				return false;
			Push(item);
			return true;
		}

		public (bool success, T item) TryPop()
		{
			if (Count > 0)
				return (true, Pop());
			return (false, default(T));
		}

		/// <summary>
		/// Returns newest item without removing it;
		/// </summary>
		/// <returns></returns>
		public T Peep()
		{
			if (IsEmpty)
				throw new InvalidOperationException("Stack empty.");
			return _items[_top];
		}

		/// <summary>
		/// Tries to return newest item without removing it;
		/// </summary>
		/// <returns>(true, item) if successful, and (false, null) if stack is empty</returns>
		public (bool success, T item) TryPeep()
		{
			if (IsEmpty)
				return (false, default(T));
			return (true, Peep());
		}

		/// <summary>
		/// Note that this will remove the enumerated items from the stack;
		/// </summary>
		/// <returns></returns>
		public IEnumerator<T> GetEnumerator()
		{
			while (!IsEmpty)
				yield return Pop();
		}

		/// <summary>
		/// Note that this will remove the enumerated items from the stack;
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
