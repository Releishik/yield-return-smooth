using System.Collections.Generic;

namespace yield
{
	public static class MovingAverageTask
	{
		public static IEnumerable<DataPoint> MovingAverage(this IEnumerable<DataPoint> data, int windowWidth)
		{
			Queue<double> window = new Queue<double>();
			int currentWatcherPosition = 0;
			double summ = 0.0;

			foreach (var point in data)
			{
				if (currentWatcherPosition >= windowWidth)
				{
					currentWatcherPosition = windowWidth;
					summ -= window.Dequeue();
				}
				window.Enqueue(point.OriginalY);
				summ += point.OriginalY;
				currentWatcherPosition++;
				yield return point.WithAvgSmoothedY(summ / window.Count);
			}
		}
	}
}