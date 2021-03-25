using System;
using System.Collections.Generic;
using System.Linq;

namespace yield
{

	public static class MovingMaxTask
	{
		public class Node
		{
			public double Value { get; set; }
			public Node Next { get; set; }
			public Node Previous { get; set; }

			public Node(Node prev, double val, Node next)
			{
				Value = val;
				Next = next;
				Previous = prev;
			}
			public Node() : this(null, double.NegativeInfinity, null)
			{ }
		}

		public class DLink
		{
			Node first;
			Node last;
			public Node First { get => first.Next; }
			public Node Last { get => last.Previous; }

			public bool IsEmpty { get => IsOnlyOne(First) && double.IsNegativeInfinity(First.Value); }

			public DLink()
			{
				var empty = new Node();
				first = new Node();
				last = new Node();
				first.Next = empty;
				last.Previous = empty;
				empty.Previous = first;
				empty.Next = last;
			}

			public bool IsOnlyOne(Node node) => node.Previous.Previous == null && node.Next.Next == null;

			public void AddFirst(double val)
			{
				if (IsEmpty)
				{
					first.Next.Value = val;
				}
				else 
				{
					AddBefore(First.Next, val);
				}
			}

			public void AddLast(double val)
			{
				if (IsEmpty) AddFirst(val);
				else
				{
					AddAfter(Last.Previous, val);
				}
			}

			public Node AddAfter(Node node, double val)
			{
				var newNode = new Node(node, val, node.Next);
				node.Next = newNode;
				newNode.Next.Previous = newNode;
				return newNode;
			}

			public Node AddBefore(Node node, double val)
			{
				var newNode = new Node(node.Previous, val, node);
				node.Previous = newNode;
				newNode.Previous.Next = newNode;
				return newNode;
			}

			public void InsertProbablyMax(double val)
			{
				if (IsEmpty) { AddFirst(val); return; }
				var node = first.Next;

				while(node!=null)
				{
					if (node.Value - val < 0)
					{
						var less = AddBefore(node, val);
						less.Next = last;
						last.Previous = less;
						break;
					}
					node = node.Next;
				}
			}

			public void RemoveFirst()
			{
				if (IsOnlyOne(first.Next)) first.Next.Value = double.NegativeInfinity;
				else
				{
					first.Next = first.Next.Next;
					first.Next.Previous = first;
				}
			}
		}

		public static IEnumerable<DataPoint> MovingMax(this IEnumerable<DataPoint> data, int windowWidth)
		{
			DLink maxList = new DLink();
			Queue<double> window = new Queue<double>();
			double bullet = double.NegativeInfinity;

			foreach(var p in data)
			{
				if (window.Count==windowWidth) bullet = window.Dequeue();
				if (bullet == maxList.First.Value) maxList.RemoveFirst();
				maxList.InsertProbablyMax(p.OriginalY);
				window.Enqueue(p.OriginalY);
				yield return p.WithMaxY(maxList.First.Value);
			}
		}
	}
}