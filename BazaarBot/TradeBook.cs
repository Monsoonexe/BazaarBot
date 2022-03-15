using System;
using System.Collections.Generic;
using System.Data;

namespace BazaarBot
{
    class TradeBook
    {
	    public readonly Dictionary<string, List<Offer>> bids;
	    public readonly Dictionary<string, List<Offer>> asks;

        public DataTable dbook { get; private set; }

	    public TradeBook()
	    {
		    bids = new Dictionary<string, List<Offer>>();
		    asks = new Dictionary<string, List<Offer>>();
            dbook = new DataTable("Book");
            dbook.Columns.Add(new DataColumn("bid"));
            dbook.Columns.Add(new DataColumn("ask"));
            dbook.Rows.Add(1.0,2.0);
        }

	    public void Register(String name)
	    {
		    asks[name] = new List<Offer>();
		    bids[name] = new List<Offer>();
	    }

	    public bool bid(Offer offer)
	    {
		    if (!bids.ContainsKey(offer.good))
			    return false;

		    bids[offer.good].Add(offer);
		    return true;
	    }

	    public bool ask(Offer offer)
	    {
		    if (!bids.ContainsKey(offer.good))
			    return false;

		    asks[offer.good].Add(offer);
		    return true;
	    }
    }
}
