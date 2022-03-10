using System;
using System.Collections.Generic;

namespace BazaarBot.Agents
{

    /**
     * An agent that performs the basic logic from the Doran & Parberry article
     * @author
     */
    class Agent : BasicAgent
    {
		/// <summary>
		/// lowest possible price.
		/// </summary>
		public const double MIN_PRICE = 0.01;
		public const double ProfitMarginFactor = 1.02f; //asks are fair prices:  costs + small profit

		public Agent(int id, AgentData data) : base(id, data)
	    {
	    }

		public override Offer? CreateBid(Market bazaar, String good, double limit)
	    {
            var bidPrice = 0;// determinePriceOf(good);  bids are now made "at market", no price determination needed //TODO - fix broken, hanging logic
		    var ideal = determinePurchaseQuantity(bazaar, good);

		    //can't buy more than limit
		    double quantityToBuy = ideal > limit ? limit : ideal;
		    if (quantityToBuy > 0)
		    {
			    return new Offer(id, good, quantityToBuy, bidPrice);
		    }
		    return null;
	    }

	    override public Offer? CreateAsk(Market bazaar, String commodity_, double limit_)
	    {
		    var ask_price = _inventory.QueryCost(commodity_) * ProfitMarginFactor; //asks are fair prices:  costs + small profit

            var quantity_to_sell = _inventory.QueryQuantity(commodity_);//put asks out for all inventory
            nProduct = quantity_to_sell; //TODO - overwrite? maybe subtract?

		    if (quantity_to_sell > 0)
		    {
			    return new Offer(id, commodity_, quantity_to_sell, ask_price);
		    }
		    return null;
	    }

	    override public void GenerateOffers(Market bazaar, string commodity)
	    {
		    Offer? offer;
		    if (_inventory.Surplus(commodity) >= 1) //surplus
		    {
			     offer = CreateAsk(bazaar, commodity, 1);
			     if (offer != null)
			     {
				    bazaar.Ask(offer.Value);
			     }
		    }
		    else
		    {
			    var shortage = _inventory.Shortage(commodity);
			    var space = _inventory.GetEmptySpace();
			    var unit_size = _inventory.GetCapacityFor(commodity);

			    if (shortage > 0 && space >= unit_size)
			    {
				    double limit = 0;
				    if ((shortage * unit_size) <= space)	//enough space for ideal order
				    {
					    limit = shortage;
				    }
				    else									//not enough space for ideal order
				    {
                        limit = space; // Math.Floor(space / shortage);
				    }

				    if (limit > 0)
				    {
					    offer = CreateBid(bazaar, commodity, limit);
					    if (offer != null)
					    {
						    bazaar.Bid(offer.Value);
					    }
				    }
			    }
		    }
	    }

	    override public void UpdatePriceModel(Market bazaar, string act, string good, bool success, double unitPrice= 0)
	    {
		    if (success)
		    {
				//Add this to my list of observed trades
				List<double> observed_trades = _observedTradingRange[good];
			    observed_trades.Add(unitPrice);
		    }

		    var public_mean_price = bazaar.GetAverageHistoricalPrice(good, 1); //TODO - do something with this value

	    }
    }
}
