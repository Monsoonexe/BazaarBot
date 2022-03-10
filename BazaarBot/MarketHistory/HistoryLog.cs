using System.Collections.Generic;

namespace BazaarBot.MarketHistory
{
	class HistoryLog
	{
		public readonly EconNoun Type;
		private readonly Dictionary<string, List<double>> log;

		public HistoryLog(EconNoun type)
		{
			Type = type;
			log = new Dictionary<string, List<double>>();
		}

		/**
	     * Add a new entry to this log
	     * @param	name
	     * @param	amount
	     */
		public void Add(string name, double amount)
		{
			if (log.TryGetValue(name, out List<double> list))
				list.Add(amount);
		}

		/**
	     * Register a new category list in this log
	     * @param	name
	     */
		public void Register(string name)
		{
			if (!log.ContainsKey(name))
			{
				log[name] = new List<double>();
			}
		}

		/**
	     * Returns the average amount of the given category, looking backwards over a specified range
	     * @param	name the category of thing
	     * @param	range how far to look back
	     * @return
	     */
		public double Average(string name, int range)
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
