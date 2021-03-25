using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace yield
{
	[TestFixture]
	public class MovingMaxTests
	{
		[Test]
		public void EmptySequence()
		{
			CheckMax(10, new double[] { }, new double[] { });
		}

		[Test]
		public void SingleZero()
		{
			CheckMax(10, new[] { 0.0 }, new[] { 0.0 });
		}

		[Test]
		public void SingleNonZeroValue()
		{
			CheckMax(10, new[] { 100.0 }, new[] { 100.0 });
		}

		[Test]
		public void TwoValuesIncreasing()
		{
			CheckMax(2, new[] { 1, 20.0 }, new[] { 1, 20.0 });
		}

		[Test]
		public void TwoValuesDecreasing()
		{
			CheckMax(2, new[] { 10, 0.0 }, new[] { 10, 10.0 });
		}

		[Test]
		public void TwoValuesWithSmallWindow()
		{
			CheckMax(1, new[] { 10, 0.0 }, new[] { 10, 0.0 });
		}

		[Test]
		public void WithWindow2()
		{
			CheckMax(2, new double[] { 1, 2, 5, 1, 0, 6 }, new double[] { 1, 2, 5, 5, 1, 6});
		}

		[Test]
		public void SmoothWithLargeWindow()
		{
			CheckMax(100500, new double[] { 1, 2, 5, 1, 0, 6 }, new double[] { 1, 2, 5, 5, 5, 6 });
		}

		[Test]
		public void RandomTest()
		{
			var rnd = new Random();
			var windowSize = rnd.Next(1, 6);
			var arr = new double[10];
			for (var i = 0; i < 10; i++)
			{
				arr[i] = rnd.Next(0, 10);
			}

			var answer = new double[arr.Length];
			for (var k = 0; k < windowSize; k++)
			{
				var max = -1d;
				for (var j = 0; j <= k; j++)
				{
					if (arr[j] > max)
					{
						max = arr[j];
					}
				}
				answer[k] = max;
			}
			for (var i = 1; i < answer.Length - windowSize + 1; i++)
			{
				var temp = arr[i];
				for (var j = i; j < windowSize + i; j++)
				{
					if (arr[j] > temp)  // Can flip inequality to get minimum
						temp = arr[j];
				}
				answer[i + windowSize - 1] = temp;
			}
			Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
			foreach (var element in arr)
			{
				Trace.Write(element);
				Trace.Write(" ");
			}
			Trace.WriteLine("");
			foreach (var element in answer)
			{
				Trace.Write(element);
				Trace.Write(" ");
			}
			Trace.WriteLine("");
			Trace.WriteLine(windowSize);
			CheckMax(windowSize, arr, answer);
		}
		private void CheckMax(int windowWidth, double[] ys, double[] expectedYs)
		{
			var dataPoints = ys.Select((v, index) => new DataPoint(GetX(index), v));
			var actual = Factory.CreateAnalyzer().MovingMax(dataPoints, windowWidth).ToList();
			Assert.AreEqual(ys.Length, actual.Count);
			for (int i = 0; i < actual.Count; i++)
			{
				Assert.AreEqual(GetX(i), actual[i].X, 1e-7);
				Assert.AreEqual(ys[i], actual[i].OriginalY, 1e-7);
				Assert.AreEqual(expectedYs[i], actual[i].MaxY, 1e-7);
			}
		}

		private double GetX(int index)
		{
			return (index - 3.0) / 2;
		}
	}
}