using System;
using System.Collections.Generic;

namespace BazaarBot.Agents
{
    abstract class AAgent
    {
		private const int DEFAULT_LOOKBACK = 15;

	    public int ID { get; set; }			//unique integer identifier
        public string ClassName { get; set; }	//string identifier, "famer", "woodcutter", etc.
        public double Money { get; set; }
        public double Space { get => _inventory.GetEmptySpace(); }
        public double nProduct { get; set; }

        //public var moneyLastRound(default, null):double;
        //public var profit(get, null):double;
        //public var inventorySpace(get, null):double;
        //public var inventoryFull(get, null):Bool;
        //public var destroyed(default, null):Bool;
        public bool destroyed; //dfs stub  needed?
        public double moneyLastRound; //dfs stub needed?
        public double profit; //dfs stub needed? //TODO - fill

        public double trackcosts;

	    /********PRIVATE************/

	    private Logic logic;
	    protected Inventory _inventory;
	    protected Dictionary<Good, List<double>> observedTradingRange;
	    private double _profit = 0;	//profit from last round
	    private int _lookback = 15;

		public AAgent(int id, string className, double money, Inventory inventory, Logic logic, int? lookback)
		{
			this.ID = id;
			this.ClassName = className;
			this.Money = money;
			this._inventory = inventory;
			this.logic = logic;
			this._lookback = lookback.HasValue ? lookback.Value : DEFAULT_LOOKBACK;

			observedTradingRange = new Dictionary<Good, List<double>>();
			trackcosts = 0;
		}

	    public AAgent(int id, AgentData data)
	    {
		    this.ID = id;
		    ClassName = data.ClassName;
		    Money = data.Money;
		    _inventory = new Inventory();
		    _inventory.FromData(data.inventory);
		    logic = data.logic;
			this._lookback = data.lookBack.HasValue ? data.lookBack.Value : DEFAULT_LOOKBACK;

			observedTradingRange = new Dictionary<Good, List<double>>();
			trackcosts = 0;
		}

	    public void Destroy()
	    {
		    destroyed = true;
		    _inventory.Destroy();
		    foreach (Good key in observedTradingRange.Keys)
		    {
			    var list = observedTradingRange[key];
                list.Clear();
		    }
            observedTradingRange.Clear();
		    observedTradingRange = null;
		    logic = null;
	    }

	    public void Init(Market market)
	    {
			List<Good> listGoods = market.GoodsTypes;
			foreach (Good str in listGoods)
		    {
			    var trades = new List<double>();

				//TODO - guess price
                var price = 2;// market.getAverageHistoricalPrice(str, _lookback);
			    trades.Add(price * 1.0);
			    trades.Add(price * 3.0);	//push two fake trades to generate a range

			    //set initial price belief & observed trading range
			    observedTradingRange[str]=trades;
		    }
	    }

	    public void Simulate(Market market)
	    {
		    logic.perform(this, market);
	    }

		public abstract void GenerateOffers(Market bazaar, Good good);

		public abstract void UpdatePriceModel(Market bazaar, string act, Good good, bool success, double unitPrice = 0);

		public abstract Offer? CreateBid(Market bazaar, GoodStack stack);

		public abstract Offer? CreateAsk(Market bazaar, GoodStack stack);

	    public double QueryQuantity(Good good) => _inventory.QueryQuantity(good);

		public void ProduceInventory(GoodStack stack)
			=> ProduceInventory(stack.Good, stack.Quantity);

		public void ProduceInventory(Good good, double delta)
		{
			if (trackcosts < 1) trackcosts = 1;
			double curunitcost = _inventory.Change(good, delta, trackcosts / delta);
			trackcosts = 0;
		}

		public void ConsumeInventory(GoodStack stack)
			=> ConsumeInventory(stack.Good, stack.Quantity);

		public void ConsumeInventory(Good good, double delta)
		{
			if (good.ID == "money")
			{
				Money += delta;
				if (delta < 0)
					trackcosts += (-delta);
			}
			else
			{
				double currentCost = _inventory.Change(good, delta, 0);
				if (delta < 0)
					trackcosts += (-delta) * currentCost;
			}
		}

		public void ChangeInventory(GoodStack stack, double unitCost)
			=> ChangeInventory(stack.Good, stack.Quantity, unitCost);

		public void ChangeInventory(Good good, double delta, double unit_cost)
		{
			if (good.ID == "money")
			{
				Money += delta;
			}
			else
			{
				_inventory.Change(good, delta, unit_cost);
			}
		}

		/********PRIVATE************/

		public bool get_inventoryFull()
	    {
		    return _inventory.GetEmptySpace() == 0;
	    }

	    public double get_profit()
	    {
		    return profit = Money - moneyLastRound;
	    }

	    protected double determineSaleQuantity(Market bazaar, Good commodity_)
	    {
		    var mean = bazaar.GetAverageHistoricalPrice(commodity_, _lookback); //double
		    var trading_range = ObserveTradingRange(commodity_, 10);//point
		    if (trading_range != null && mean>0)
		    {
			    var favorability= Quick.positionInRange(mean, trading_range.Value.x, trading_range.Value.y);//double
			    //position_in_range: high means price is at a high point

				//TODO - fix logic
			    double amount_to_sell = Math.Round(favorability * _inventory.Surplus(commodity_)); //double
				amount_to_sell = _inventory.QueryQuantity(commodity_);
			    if (amount_to_sell < 1)
			    {
				    amount_to_sell = 1;
			    }
			    return amount_to_sell;
		    }
		    return 0;
	    }

        protected double DeterminePurchaseQuantity(Market bazaar, Good commodity_)
	    {
		    var mean = bazaar.GetAverageHistoricalPrice(commodity_, _lookback);//double
		    var trading_range = ObserveTradingRange(commodity_, 10); //Point
		    if (trading_range != null)
		    {
			    var favorability = Quick.positionInRange(mean, trading_range.Value.x, trading_range.Value.y);//double
			    favorability = 1 - favorability;
			    //do 1 - favorability to see how close we are to the low end

			    double amount_to_buy = Math.Round(favorability * _inventory.Shortage(commodity_));//double
			    if (amount_to_buy < 1)
			    {
				    amount_to_buy = 1;
			    }
			    return amount_to_buy;
		    }
		    return 0;
	    }

	    private Point? ObserveTradingRange(Good good, int window)
	    {
		    List<double> a = observedTradingRange[good]; 
		    Point pt = new Point(Quick.minArr(a,window), Quick.maxArr(a,window));
		    return pt;
	    }
    }
}
