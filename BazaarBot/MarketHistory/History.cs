using System;
using System.Collections.Generic;

namespace EconomySim.MarketHistory
{
    class HistoryLog
    {
	    public readonly EconNoun type;
	    private readonly Dictionary<string, List<double>> log;

	    public HistoryLog(EconNoun type)
	    {
		    this.type = type;
		    log = new Dictionary<string, List<double>>();
	    }

	    /**
	     * Add a new entry to this log
	     * @param	name
	     * @param	amount
	     */
	    public void add(string name, double amount)
	    {
		    if (log.ContainsKey(name))
		    {
			    var list = log[name];
			    list.Add(amount);
		    }
	    }

	    /**
	     * Register a new category list in this log
	     * @param	name
	     */
	    public void register(String name)
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
	    public double Average(String name, int range)
	    {
		    if (log.ContainsKey(name))
		    {
			    var list = log[name];
			    double amt = 0.0;
			    var length = list.Count;
			    if (length < range)
			    {
				    range = length;
			    }
			    for (int i=0; i<range; i++)
			    {
				    amt += list[length - 1 - i];
			    }
                if (range <= 0) return -1;
			    return amt / range;
		    }
		    return 0;
	    }
    }

    class History
    {
	    public HistoryLog prices;
	    public HistoryLog asks;
	    public HistoryLog bids;
	    public HistoryLog trades;
	    public HistoryLog profit;

	    public History()
	    {
		    prices = new HistoryLog(EconNoun.Price);
		    asks   = new HistoryLog(EconNoun.Ask);
		    bids   = new HistoryLog(EconNoun.Bid);
		    trades = new HistoryLog(EconNoun.Trade);
		    profit = new HistoryLog(EconNoun.Profit);
	    }

	    public void register(string good)
	    {
		    prices.register(good);
		    asks.register(good);
		    bids.register(good);
		    trades.register(good);
		    profit.register(good);
	    }
    }
}
