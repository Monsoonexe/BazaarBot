using System;
using System.Collections.Generic;

namespace BazaarBot.Agents
{

    /**
     * An agent that performs the basic logic from the Doran & Parberry article
     * @author
     */
    class Agent : AAgent
    {
		/// <summary>
		/// lowest possible price.
		/// </summary>
		public const double MIN_PRICE = 0.01;
		public const double ProfitMarginFactor = 1.02f; //asks are fair prices:  costs + small profit

		public Agent(int id, AgentData data) : base(id, data)
	    {
	    }

		public override Offer? CreateBid(Market bazaar, GoodStack stack)
	    {
			double limit = stack.Quantity;
            var bidPrice = 0;// determinePriceOf(good);  bids are now made "at market", no price determination needed //TODO - fix broken, hanging logic
		    var ideal = DeterminePurchaseQuantity(bazaar, stack.Good);

		    //can't buy more than limit
		    double quantityToBuy = ideal > limit ? limit : ideal;
		    if (quantityToBuy > 0)
		    {
			    return new Offer(ID, stack.Good, quantityToBuy, bidPrice);
		    }
		    return null;
	    }

		public override Offer? CreateAsk(Market bazaar, GoodStack stack)
	    {
		    var ask_price = _inventory.QueryCost(stack.Good) * ProfitMarginFactor; //asks are fair prices:  costs + small profit

            var quantity_to_sell = _inventory.QueryQuantity(stack.Good);//put asks out for all inventory
            nProduct = quantity_to_sell; //TODO - overwrite? maybe subtract?

		    if (quantity_to_sell > 0)
		    {
			    return new Offer(ID, stack.Good, quantity_to_sell, ask_price);
		    }
		    return null;
	    }

	    public override void GenerateOffers(Market bazaar, Good commodity)
	    {
		    Offer? offer;
		    if (_inventory.Surplus(commodity) >= 1) //surplus
		    {
			     offer = CreateAsk(bazaar, new GoodStack(commodity, 1));
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
					    offer = CreateBid(bazaar, new GoodStack(commodity, limit));
					    if (offer != null)
					    {
						    bazaar.Bid(offer.Value);
					    }
				    }
			    }
		    }
	    }

		public override void UpdatePriceModel(Market bazaar, string act, Good good, bool success, double unitPrice= 0)
	    {
		    if (success)
		    {
				//Add this to my list of observed trades
				List<double> observed_trades = observedTradingRange[good];
			    observed_trades.Add(unitPrice);
		    }

		    var public_mean_price = bazaar.GetAverageHistoricalPrice(good, 1); //TODO - do something with this value

	    }
    }
}
