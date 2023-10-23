using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BazaarBot.MarketHistory;
using BazaarBot.Agents;

namespace BazaarBot
{
    interface ISignalBankrupt
    {
        void signalBankrupt(Market market, AAgent agent);
    }

    internal class Market
    {
	    public string name;

	    /**Logs information about all economic activity in this market**/
	    public History history;

	    /**Signal fired when an agent's money reaches 0 or below**/
	    public ISignalBankrupt signalBankrupt;

	    /********PRIVATE*********/

	    private int _roundNum = 0;

		/// <summary>
		/// list of all the legal commodities
		/// </summary>
		private List<Good> goodTypes;
		public List<Good> GoodsTypes => goodTypes;
	    public BindingList<AAgent> _agents;
	    public readonly TradeBook Book;
	    private Dictionary<string, AgentData> _mapAgents;
	    private Dictionary<string, GoodStack> _mapGoods;
        
        public Market(string name, ISignalBankrupt isb)
	    {
		    this.name = name;

		    history = new History();
		    Book = new TradeBook();
		    goodTypes = new List<Good>();
		    _agents = new BindingList<AAgent>();
		    _mapGoods = new Dictionary<string, GoodStack>();
		    _mapAgents = new Dictionary<string, AgentData>();

		    signalBankrupt = isb;//new TypedSignal<Market->BasicAgent->Void>();
	    }

	    public void init(MarketData data)
	    {
		    FromData(data);
	    }

	    public int numTypesOfGood()
	    {
		    return goodTypes.Count;
	    }

	    public int numAgents()
	    {
		    return _agents.Count;
	    }

	    public void replaceAgent(AAgent oldAgent, AAgent newAgent)
	    {
		    newAgent.ID = oldAgent.ID;
		    _agents[oldAgent.ID] = newAgent;
		    oldAgent.Destroy();
		    newAgent.Init(this);
	    }

	    public void Simulate(int rounds)
	    {
		    for (int round=0; round<rounds; round++)
		    {
			    foreach (var agent in _agents)
			    {
				    agent.moneyLastRound = agent.Money;
				    agent.Simulate(this);

				    foreach (var commodity in goodTypes)
				    {
					    agent.GenerateOffers(this, commodity);
				    }
			    }

			    foreach (var commodity in goodTypes)
			    {
				    ResolveOffers(commodity);
			    }
                var del = new List<AAgent>();
			    foreach (var agent in _agents)
			    {
                    if (agent.Money <= 0) del.Add(agent);  
			    }
                while (del.Count > 0)
                {
                    signalBankrupt.signalBankrupt(this, del[0]); //signalBankrupt.dispatch(this, agent);
                    del.RemoveAt(0);
                }
                _roundNum++;
		    }
	    }

	    public void Ask(Offer offer)
	    {
		    Book.ask(offer);
	    }

	    public void Bid(Offer offer)
	    {
		    Book.bid(offer);
	    }

	    /**
	     * Returns the historical mean price of the given commodity over the last X rounds
	     * @param	commodity_ string id of commodity
	     * @param	range number of rounds to look back
	     * @return
	     */

	    public double GetAverageHistoricalPrice(Good good, int range)
		    => history.Prices.Average(good, range);

	    /**
	     * Get the good with the highest demand/supply ratio over time
	     * @param   minimum the minimum demand/supply ratio to consider an opportunity
	     * @param	range number of rounds to look back
	     * @return
	     */

	    public Good GetHottestGood(double minimum = 1.5, int range = 10)
	    {
		    Good best_market = null;
			double best_ratio = double.MinValue;
		    foreach (var good in goodTypes)
		    {
			    var asks = history.Asks.Average(good, range);
			    var bids = history.Bids.Average(good, range);

			    double ratio = 0;
			    if (asks == 0 && bids > 0)
			    {
				    //If there are NONE on the market we artificially create a fake supply of 1/2 a unit to avoid the
				    //crazy bias that "infinite" demand can cause...
				    asks = 0.5;
			    }

			    ratio = bids / asks;

			    if (ratio > minimum && ratio > best_ratio)
			    {
				    best_ratio = ratio;
				    best_market = good;
			    }
		    }
		    return best_market;
	    }

	    /**
	     * Returns the good that has the lowest average price over the given range of time
	     * @param	range how many rounds to look back
	     * @param	exclude goods to exclude
	     * @return
	     */

	    public Good GetCheapestGood(int range, List<Good> exclude = null)
	    {
            double best_price = double.MinValue;
		    Good best_good = null;
		    foreach (var g in goodTypes)
		    {
			    if (exclude == null || !exclude.Contains(g))
			    {
				    double price = history.Prices.Average(g, range);
				    if (price < best_price)
				    {
					    best_price = price;
					    best_good = g;
				    }
			    }
		    }
		    return best_good;
	    }

		/// <summary>
		/// Returns the good that has the highest average price over the given range of time.
		/// </summary>
		/// <param name="range">how many rounds to look back</param>
		/// <param name="exclude">goods to exclude</param>
		/// <returns></returns>
		public Good GetDearestGood(int range, List<Good> exclude= null)
	    {
		    double best_price = 0;
			Good best_good = null;
		    foreach (var g in goodTypes)
		    {
			    if (exclude == null || !exclude.Contains(g))
			    {
				    var price = history.Prices.Average(g, range);
				    if (price > best_price)
				    {
					    best_price = price;
					    best_good = g;
				    }
			    }
		    }
		    return best_good;
	    }

	    /**
	     *
	     * @param	range
	     * @return
	     */
	    public Good GetMostProfitableAgentClass(int range= 10)
	    {
            double best = double.MinValue;
			Good bestClass = null;
		    foreach (var className in _mapAgents.Keys)
		    {
			    double val = history.Profit.Average(className, range);
			    if (val > best)
			    {
				    bestClass = new Good(className);
				    best = val;
			    }
		    }
		    return bestClass;
	    }

	    public AgentData GetAgentClass(Good good)
		    => _mapAgents[good.Id];

		/// <summary>
		/// Get the <see cref="Good"/>s that an <see cref="Agent"/> uses.
		/// </summary>
		/// <returns></returns>
	    public List<string> GetAgentClassNames()
	    {
		    var agentData = new List<string>();
		    foreach (var key in _mapAgents.Keys)
		    {
			    agentData.Add(key);
		    }
		    return agentData;
	    }

	    /********REPORT**********/
	    public MarketReport get_marketReport(int rounds)
	    {
		    var mr = new MarketReport();
		    mr.strListGood = "Commodities\n\n";
		    mr.strListGoodPrices = "Price\n\n";
		    mr.strListGoodTrades = "Trades\n\n";
		    mr.strListGoodAsks = "Supply\n\n";
		    mr.strListGoodBids = "Demand\n\n";

		    mr.strListAgent = "Classes\n\n";
		    mr.strListAgentCount = "Count\n\n";
		    mr.strListAgentProfit = "Profit\n\n";
		    mr.strListAgentMoney = "Money\n\n";

		    mr.arrStrListInventory = new List<String>();

		    foreach (var commodity in goodTypes)
		    {
			    mr.strListGood += commodity + "\n";

			    var price = history.Prices.Average(commodity, rounds);
			    mr.strListGoodPrices += Quick.numStr(price, 2) + "\n";

			    var asks = history.Asks.Average(commodity, rounds);
			    mr.strListGoodAsks += (int)(asks) + "\n";

			    var bids = history.Bids.Average(commodity, rounds);
			    mr.strListGoodBids += (int)(bids) + "\n";

			    var trades = history.Trades.Average(commodity, rounds);
			    mr.strListGoodTrades += (int)(trades) + "\n";

			    mr.arrStrListInventory.Add(commodity + "\n\n");
		    }
		    foreach (var key in _mapAgents.Keys)
		    {
			    var inventory = new List<double>();
			    foreach (var str in goodTypes)
			    {
				    inventory.Add(0);
			    }
			    mr.strListAgent += key + "\n";
			    var profit = history.Profit.Average(key, rounds);
			    mr.strListAgentProfit += Quick.numStr(profit, 2) + "\n";

			    var list = _agents; //var list = _agents.filter(function(a:BasicAgent):Bool { return a.className == key; } );  dfs stub wtf
			    int count = 0;
			    double money = 0;

			    foreach (var a in list)
			    {
                    if (a.ClassName == key)
                    {
                        count++;
				        money += a.Money;
				        for (int lic=0; lic<goodTypes.Count; lic++)
				        {
					        inventory[lic] += a.QueryQuantity(goodTypes[lic]);
				        }
                    }
			    }

			    money /= count;
			    for (int lic =0; lic<goodTypes.Count; lic++)
			    {
				    inventory[lic] /= count;
				    mr.arrStrListInventory[lic] += Quick.numStr(inventory[lic],1) + "\n";
			    }

			    mr.strListAgentCount += Quick.numStr(count, 0) + "\n";
			    mr.strListAgentMoney += Quick.numStr(money, 0) + "\n";
		    }
		    return mr;
	    }

	    /********PRIVATE*********/

	    private void FromData(MarketData data)
	    {
		    //Create commodity index
		    foreach (var stack in data.goods)
		    {
			    goodTypes.Add(stack.Good);
			    _mapGoods[stack.Good.Id] = new GoodStack(stack.Good, stack.Quantity);

                double v = 1.0;
                if (stack.Good.Id == "metal") v = 2.0;
                if (stack.Good.Id == "tools") v = 3.0;

			    history.Register(stack.Good);
                history.Prices.Add(stack.Good, v);	//start the bidding at $1!
                history.Asks.Add(stack.Good, v);	//start history charts with 1 fake buy/sell bid
                history.Bids.Add(stack.Good, v);
                history.Trades.Add(stack.Good, v);

			    Book.Register(stack.Good);
		    }

		    _mapAgents = new Dictionary<string, AgentData>();

		    foreach (var agentData in data.agentTypes)
		    {
			    _mapAgents[agentData.ClassName] = agentData;
			    history.Profit.Register(new Good(agentData.ClassName));
		    }

		    //Make the agent list
		    _agents = new BindingList<AAgent>();

		    var agentIndex = 0;
		    foreach (var agent in data.agents)
		    {
			    agent.ID = agentIndex;
			    agent.Init(this);
			    _agents.Add(agent);
			    agentIndex++;
		    }
	    }

	    private void ResolveOffers(Good good)
	    {
		    var bids = Book.bids[good];
		    var asks = Book.asks[good];

			bids.Shuffle();
			asks.Shuffle();

			bids.Sort(Quick.sortOfferDecending); //highest buying price first
			//asks.Sort(Quick.sortOfferAcending); //lowest selling price first

		    int successfulTrades = 0;		//# of successful trades this round
		    double moneyTraded = 0;			//amount of money traded this round
		    double unitsTraded = 0;			//amount of goods traded this round
		    double avgPrice = 0;				//avg clearing price this round
		    double numAsks = 0;
		    double numBids = 0;

		    int failsafe = 0;

		    for (int i=0; i<bids.Count; i++)
		    {
			    numBids += bids[i].units;
		    }

		    for (int i=0; i<asks.Count; i++)
		    {
			    numAsks += asks[i].units;
		    }

		    //march through and try to clear orders
		    while (bids.Count > 0 && asks.Count > 0)		//while both books are non-empty
		    {
			    Offer buyer = bids.Last();
			    Offer seller = asks.Last();

			    double quantity_traded = Math.Min(seller.units, buyer.units);
                double clearing_price = seller.unit_price; //Quick.avgf(seller.unit_price, buyer.unit_price);

                //if (buyer.unit_price < seller.unit_price)
                //    break;

			    if (quantity_traded > 0)
			    {
				    //transfer the goods for the agreed price
				    seller.units -= quantity_traded;
				    buyer.units -= quantity_traded;

				    TransferGood(good, quantity_traded, seller.agent_id, buyer.agent_id, clearing_price);
				    TransferMoney(quantity_traded * clearing_price, seller.agent_id, buyer.agent_id);

				    //update agent price beliefs based on successful transaction
				    var buyer_a = _agents[buyer.agent_id];
				    var seller_a = _agents[seller.agent_id];
				    buyer_a.UpdatePriceModel(this, "buy", good, true, clearing_price);
				    seller_a.UpdatePriceModel(this, "sell", good, true, clearing_price);

				    //log the stats
				    moneyTraded += (quantity_traded * clearing_price);
				    unitsTraded += quantity_traded;
				    successfulTrades++;
			    }

			    if (seller.units == 0)		//seller is out of offered good
			    {
				    asks.RemoveLast(); //.splice(0, 1);		//remove ask
				    failsafe = 0;
			    }
			    if (buyer.units == 0)		//buyer is out of offered good
			    {
				    bids.RemoveLast();//.splice(0, 1);		//remove bid
				    failsafe = 0;
			    }

			    failsafe++;

			    if (failsafe > 1000)
			    {
				    Console.WriteLine($"Failsafe hit after {failsafe} iterations!");
					break;
			    }
		    }

		    //reject all remaining offers,
		    //update price belief models based on unsuccessful transaction
		    while (bids.Count > 0)
		    {
				Offer buyer = bids.GetRemoveLast();
				AAgent buyer_a = _agents[buyer.agent_id];
			    buyer_a.UpdatePriceModel(this,"buy",good, false);
		    }
            while (asks.Count > 0)
		    {
			    var seller = asks.GetRemoveLast();
			    var seller_a = _agents[seller.agent_id];
			    seller_a.UpdatePriceModel(this,"sell",good, false);
		    }

		    //update history

		    history.Asks.Add(good, numAsks);
		    history.Bids.Add(good, numBids);
		    history.Trades.Add(good, unitsTraded);

		    if (unitsTraded > 0)
		    {
			    avgPrice = moneyTraded / (double)unitsTraded;
			    history.Prices.Add(good, avgPrice);
		    }
		    else
		    {
			    //special case: none were traded this round, use last round's average price
			    history.Prices.Add(good, history.Prices.Average(good, 1));
			    avgPrice = history.Prices.Average(good,1);
		    }

            List<AAgent> ag = _agents.ToList<AAgent>();
		    ag.Sort(Quick.sortAgentAlpha);

		    string curr_class = "";
		    string last_class = "";
		    List<double> list  = null;

		    for (int i = 0; i < ag.Count; i++)
		    {
			    var a = ag[i];		//get current agent
			    curr_class = a.ClassName;			//check its class
			    if (curr_class != last_class)		//new class?
			    {
				    if (list != null)				//do we have a list built up?
				    {
					    //log last class' profit
					    history.Profit.Add(new Good(last_class), list.Average());
				    }
				    list = new List<double>();		//make a new list
				    last_class = curr_class;
			    }
			    list.Add(a.get_profit());			//push profit onto list
		    }

		    //add the last class too
		    history.Profit.Add(new Good(last_class), list.Average());
	    }

	    private void TransferGood(Good good, double units, int seller_id, int buyer_id, double clearing_price)
	    {
		    var seller = _agents[seller_id];
		    var buyer = _agents[buyer_id];
		    seller.ChangeInventory(good, -units, 0);
		    buyer.ChangeInventory(good, units, clearing_price);
	    }

	    private void TransferMoney(double amount, int seller_id, int buyer_id)
	    {
		    var seller = _agents[seller_id];
		    var buyer = _agents[buyer_id];
		    seller.Money += amount;
		    buyer.Money -= amount;
	    }

    }
}
