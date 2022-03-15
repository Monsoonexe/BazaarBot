using System.Collections.Generic;

namespace BazaarBot.MarketHistory
{
	class HistoryLog
	{
		public readonly EconNoun Type;
		private readonly Dictionary<Good, List<double>> log;

		public HistoryLog(EconNoun type)
		{
			Type = type;
			log = new Dictionary<Good, List<double>>();
		}

		/**
	     * Add a new entry to this log
	     * @param	name
	     * @param	amount
	     */
		public void Add(Good name, double amount)
		{
			if (log.TryGetValue(name, out List<double> list))
				list.Add(amount);
		}

		/**
	     * Register a new category list in this log
	     * @param	name
	     */
		public void Register(Good name)
		{
			if (!log.ContainsKey(name))
			{
				log[name] = new List<double>();
			}
		}

		/// <summary>
		/// Returns the average amount of the given category, looking backwards over a specified range
		/// </summary>
		/// <param name="name">the category of <see cref="Good"/>.</param>
		/// <param name="range">how far to look back</param>
		/// <returns></returns>
		public double Average(string name, int range)
			=> Average(new Good(name), range);

		/// <summary>
		/// Returns the average amount of the given category, looking backwards over a specified range
		/// </summary>
		/// <param name="name">the category of <see cref="Good"/>.</param>
		/// <param name="range">how far to look back</param>
		/// <returns></returns>
		public double Average(Good name, int range)
		{
			if (log.TryGetValue(name, out List<double> list))
			{
				double amt = 0.0;
				var length = list.Count;
				if (length < range)
				{
					range = length;
				}
				for (int i = 0; i < range; i++)
				{
					amt += list[length - 1 - i];
				}
				return (range <= 0) ? -1 : amt / range;
			}
			return 0;
		}
	}
}
