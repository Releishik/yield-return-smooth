using System.Collections.Generic;

namespace yield
{
	public static class ExpSmoothingTask
	{
		public static IEnumerable<DataPoint> SmoothExponentialy(this IEnumerable<DataPoint> data, double alpha)
		{
			double s = double.NegativeInfinity;
			double lastS = s;

			foreach (var point in data)
			{
				if (double.IsNegativeInfinity(s))
				{
					s = point.OriginalY;
					lastS = s;
					yield return point.WithExpSmoothedY(s);
				}
				else
				{
					s = lastS + alpha * (point.OriginalY - lastS);
					lastS = s;
					yield return point.WithExpSmoothedY(s);
				}
			}
		}
	}
}