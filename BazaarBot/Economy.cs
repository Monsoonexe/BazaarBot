using System.Collections.Generic;
using BazaarBot.Agents;

namespace BazaarBot
{
    class Economy : ISignalBankrupt
    {
	    private readonly List<Market> markets;

        public Economy()
	    {
		    markets = new List<Market>();
	    }

	    public void AddMarket(Market m)
			=> markets.AddIfNew(m);

	    public Market GetMarket(string name)
	    {
		    foreach (var m in markets)
		    {
			    if (m.name == name) return m;
		    }
		    return null;
	    }

	    public void SimulateMarkets(int rounds)
	    {
		    foreach (var m in markets)
			    m.Simulate(rounds);
	    }

	    public virtual void signalBankrupt(Market m, AAgent a)
	    {
		    //no implemenation -- provide your own in a subclass
	    }

    }

}
